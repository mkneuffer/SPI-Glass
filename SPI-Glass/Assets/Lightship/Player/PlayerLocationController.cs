using System;
using System.Collections;
using Niantic.Lightship.Maps.Core.Coordinates;
using UnityEngine;
using UnityEngine.UI;  // Add this to work with UI elements

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace Niantic.Lightship.Maps.SampleAssets.Player
{
    public class PlayerLocationController : MonoBehaviour
    {
        [SerializeField]
        private LightshipMapView _lightshipMapView;

        [SerializeField]
        private float _editorMovementSpeed;

        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private PlayerModel _model;

        [SerializeField]
        private Toggle useEditorControlsToggle;  // Reference to the UI Toggle

        private double _lastGpsUpdateTime;
        private Vector3 _targetMapPosition;
        private Vector3 _currentMapPosition;
        private float _lastMapViewUpdateTime;

        // Track if UI buttons are held down
        private bool _isMovingUp = false;
        private bool _isMovingDown = false;
        private bool _isMovingLeft = false;
        private bool _isMovingRight = false;

        public Action<string> OnGpsError;

        private const float WalkThreshold = 0.5f;
        private const float TeleportThreshold = 200f;

        // Boolean to track if we should use editor controls
        private bool useEditorControls;

        private static bool IsLocationServiceInitializing
            => Input.location.status == LocationServiceStatus.Initializing;

        private void Start()
        {
            _lightshipMapView.MapOriginChanged += OnMapViewOriginChanged;
            _currentMapPosition = _targetMapPosition = transform.position;

            // Set initial value of useEditorControls based on the toggle's state
            useEditorControls = useEditorControlsToggle.isOn;

            // Add a listener to update useEditorControls when the toggle is changed
            useEditorControlsToggle.onValueChanged.AddListener(OnToggleChanged);

            // Start GPS location update coroutine if not using editor controls
            if (!useEditorControls)
            {
                StartCoroutine(UpdateGpsLocation());
            }
        }

        private void OnToggleChanged(bool isOn)
        {
            // Update the useEditorControls flag based on the toggle's state
            useEditorControls = isOn;

            // If we switched to GPS, start the coroutine
            if (!useEditorControls)
            {
                StartCoroutine(UpdateGpsLocation());
            }
        }

        private void OnMapViewOriginChanged(LatLng center)
        {
            var offset = _targetMapPosition - _currentMapPosition;
            _currentMapPosition = _lightshipMapView.LatLngToScene(center);
            _targetMapPosition = _currentMapPosition + offset;
        }

        private IEnumerator UpdateGpsLocation()
        {
            yield return null;

            if (Application.isEditor && !useEditorControls)
            {
                while (isActiveAndEnabled)
                {
                    UpdateEditorInput();
                    yield return null;
                }
            }
            else
            {
#if UNITY_ANDROID
                if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                {
                    Permission.RequestUserPermission(Permission.FineLocation);
                    while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                    {
                        yield return new WaitForSeconds(1.0f);
                    }
                }
#endif
                if (!Input.location.isEnabledByUser)
                {
                    OnGpsError?.Invoke("Location permission not enabled");
                    yield break;
                }

                Input.location.Start();

                int maxWait = 20;
                while (IsLocationServiceInitializing && maxWait > 0)
                {
                    yield return new WaitForSeconds(1);
                    maxWait--;
                }

                if (maxWait < 1)
                {
                    OnGpsError?.Invoke("GPS initialization timed out");
                    yield break;
                }

                if (Input.location.status == LocationServiceStatus.Failed)
                {
                    OnGpsError?.Invoke("Unable to determine device location");
                    yield break;
                }

                while (isActiveAndEnabled && !useEditorControls)  // Only run if not using editor controls
                {
                    var gpsInfo = Input.location.lastData;
                    if (gpsInfo.timestamp > _lastGpsUpdateTime)
                    {
                        _lastGpsUpdateTime = gpsInfo.timestamp;
                        var location = new LatLng(gpsInfo.latitude, gpsInfo.longitude);
                        UpdatePlayerLocation(location);
                    }

                    yield return null;
                }

                Input.location.Stop();
            }
        }

        private void UpdatePlayerLocation(in LatLng location)
        {
            _targetMapPosition = _lightshipMapView.LatLngToScene(location);
        }

        public void Update()
        {
            UpdateMapViewPosition();

            if (useEditorControls)
            {
                UpdateEditorInput();
            }

            var movementVector = _targetMapPosition - _currentMapPosition;
            var movementDistance = movementVector.magnitude;

            switch (movementDistance)
            {
                case > TeleportThreshold:
                    _currentMapPosition = _targetMapPosition;
                    break;

                case > WalkThreshold:
                {
                    var forward = movementVector.normalized;
                    var rotation = Quaternion.LookRotation(forward, Vector3.up);
                    transform.rotation = rotation;
                    break;
                }
            }

            _currentMapPosition = Vector3.Lerp(
                _currentMapPosition,
                _targetMapPosition,
                Time.deltaTime);

            transform.position = _currentMapPosition;
            _model.UpdatePlayerState(movementDistance);

            // Check if any direction button is held down (only in editor control mode)
            if (useEditorControls)
            {
                if (_isMovingUp) { MoveCharacter(Vector3.forward); }
                if (_isMovingDown) { MoveCharacter(Vector3.back); }
                if (_isMovingLeft) { MoveCharacter(Vector3.left); }
                if (_isMovingRight) { MoveCharacter(Vector3.right); }
            }
        }

        public void StartMoveUp() { _isMovingUp = true; }
        public void StopMoveUp() { _isMovingUp = false; }

        public void StartMoveDown() { _isMovingDown = true; }
        public void StopMoveDown() { _isMovingDown = false; }

        public void StartMoveLeft() { _isMovingLeft = true; }
        public void StopMoveLeft() { _isMovingLeft = false; }

        public void StartMoveRight() { _isMovingRight = true; }
        public void StopMoveRight() { _isMovingRight = false; }

        private void MoveCharacter(Vector3 direction)
        {
            var cameraForward = _camera.transform.forward;
            float yRotation = Vector3.SignedAngle(Vector3.forward, cameraForward, Vector3.up);
            Vector3 adjustedDirection = Quaternion.AngleAxis(yRotation, Vector3.up) * direction;

            _targetMapPosition += adjustedDirection * (_editorMovementSpeed * Time.deltaTime);
        }

        private void UpdateEditorInput()
        {
            if (Input.GetKey(KeyCode.W)) { MoveCharacter(Vector3.forward); }
            if (Input.GetKey(KeyCode.S)) { MoveCharacter(Vector3.back); }
            if (Input.GetKey(KeyCode.A)) { MoveCharacter(Vector3.left); }
            if (Input.GetKey(KeyCode.D)) { MoveCharacter(Vector3.right); }
        }

        private void UpdateMapViewPosition()
        {
            if (Time.time < _lastMapViewUpdateTime + 1.0f) { return; }
            _lastMapViewUpdateTime = Time.time;
            _lightshipMapView.SetMapCenter(transform.position);
        }
    }
}
