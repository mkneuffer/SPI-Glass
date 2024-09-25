using Niantic.Lightship.AR.Utilities;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class Depth_ScreenToWorldPosition : MonoBehaviour
{
    [SerializeField]
    public AROcclusionManager _occlusionManager;
    
    [SerializeField]
    public ARCameraManager _arCameraManager;
    
    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private ARRaycastManager ARRaycastManager;
    
    [SerializeField]
    public GameObject _prefabToSpawn;
    
    private Matrix4x4 _displayMatrix;
    private XRCpuImage? _depthImage;
    
    private void OnEnable()
    {
        _arCameraManager.frameReceived += OnCameraFrameEventReceived;
    }
    
    private void OnDisable()
    {
        _arCameraManager.frameReceived -= OnCameraFrameEventReceived;
    }
    
    private void OnCameraFrameEventReceived(ARCameraFrameEventArgs args)
    {
        // Cache the screen to image transform
        if (args.displayMatrix.HasValue)
        {
#if UNITY_IOS
            _displayMatrix = args.displayMatrix.Value.transpose;
#else
            _displayMatrix = args.displayMatrix.Value;
#endif
        }
    }
    
    void Update()
    {
        if (!_occlusionManager.subsystem.running)
        {
            return;
        }
        if (_occlusionManager.TryAcquireEnvironmentDepthCpuImage(out
            var image))
        {
            // Dispose the old image
            _depthImage?.Dispose();
            _depthImage = image;
        }
        else
        {
            return;
        }

        HandleTouch();
    }
    
    private void HandleTouch()
    {
        // in the editor we want to use mouse clicks, on phones we want touches.
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            var screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
#else
        //if there is no touch or touch selects UI element
        if (Input.touchCount <= 0)
            return;
        var touch = Input.GetTouch(0);

        // only count touches that just began
        if (touch.phase == UnityEngine.TouchPhase.Began)
        {
            var screenPosition = touch.position;
#endif

            // Use the ARRaycastManager to find a valid position in the AR world
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (ARRaycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinBounds | TrackableType.FeaturePoint))
            {
                var hitPose = hits[0].pose;
                // Place the prefab at the hit position in the world space
                Instantiate(_prefabToSpawn, hitPose.position, hitPose.rotation);
            }

            else if (_depthImage.HasValue)
            {
                // If no AR raycast hit, fallback to depth map sampling
                var uv = new Vector2(screenPosition.x / Screen.width, screenPosition.y / Screen.height);
                var eyeDepth = _depthImage.Value.Sample<float>(uv, _displayMatrix);
                
                // Convert depth to world position (ensure proper depth placement)
                var ray = _camera.ScreenPointToRay(screenPosition);
                var worldPosition = ray.GetPoint(eyeDepth);

                // Spawn a thing on the depth map
                Instantiate(_prefabToSpawn, worldPosition, Quaternion.identity);
            }
        }
    }
}