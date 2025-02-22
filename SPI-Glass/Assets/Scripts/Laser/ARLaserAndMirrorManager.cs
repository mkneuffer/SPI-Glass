using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

public class ARLaserAndMirrorManager : MonoBehaviour
{
    [Header("Prefabs & Inventory")]
    public GameObject prefab1;
    public GameObject prefab2;
    public int inventoryCount1 = 5;
    public int inventoryCount2 = 5;

    public enum PrefabType { Type1, Type2 }
    [Header("Active Prefab")]
    public PrefabType currentPrefab = PrefabType.Type1;

    [Header("Rotation Settings")]
    public float rotationSpeed = 5.0f;

    private ARRaycastManager arRaycastManager;
    private static List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();

    // For drag rotation: once started on a "Rotate" collider, it continues until release
    private GameObject currentRotatingObject = null;
    private Vector2 lastTouchPosition;

    // For deletion candidate handling
    private Collider deleteCandidate = null;
    private Vector2 deleteStartPosition;
    private bool deletionTouchActive = false;
    // Threshold in pixels to differentiate a tap from a drag on the delete collider.
    private float deletionDragThreshold = 10f;

    void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        if (arRaycastManager == null)
        {
            Debug.LogError("ARRaycastManager component is required on the XR Origin.");
        }
    }

    void Update()
    {
        // Process touch input if available...
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            ProcessInput(touch.position, touch.phase);
        }
        else
        {
            // ...or use mouse input for editor testing.
            if (Input.GetMouseButtonDown(0))
            {
                ProcessInput(Input.mousePosition, TouchPhase.Began);
            }
            else if (Input.GetMouseButton(0))
            {
                ProcessInput(Input.mousePosition, TouchPhase.Moved);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                ProcessInput(Input.mousePosition, TouchPhase.Ended);
            }
        }
    }

    void ProcessInput(Vector2 screenPosition, TouchPhase phase)
    {
        // Prevent AR input when clicking on UI.
        if (EventSystem.current != null)
        {
            if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return;
            }
            if (Input.touchCount == 0 && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
        }

        // If rotation is already active, update it regardless of pointer location.
        if (currentRotatingObject != null)
        {
            if (phase == TouchPhase.Moved)
            {
                Vector2 delta = screenPosition - lastTouchPosition;
                float rotationAmount = delta.x * rotationSpeed * Time.deltaTime;
                currentRotatingObject.transform.Rotate(Vector3.up, -rotationAmount, Space.World);
                lastTouchPosition = screenPosition;
            }
            else if (phase == TouchPhase.Ended || phase == TouchPhase.Canceled)
            {
                currentRotatingObject = null;
            }
            return;
        }

        // Process new touches when no rotation is active.
        if (phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.collider.CompareTag("Delete"))
                {
                    // Begin deletion candidate.
                    deleteCandidate = hitInfo.collider;
                    deleteStartPosition = screenPosition;
                    deletionTouchActive = true;
                    return;
                }
                else if (hitInfo.collider.CompareTag("Rotate"))
                {
                    // Begin rotation. Once started, rotation continues even if pointer leaves collider.
                    currentRotatingObject = hitInfo.collider.transform.root.gameObject;
                    lastTouchPosition = screenPosition;
                    return;
                }
            }
            // If nothing interactive was hit, attempt to place an object on an AR plane.
            if (arRaycastManager.Raycast(screenPosition, raycastHits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = raycastHits[0].pose;
                PlaceObject(hitPose);
            }
        }
        else if (phase == TouchPhase.Moved)
        {
            if (deletionTouchActive)
            {
                // Cancel deletion if moved too far.
                if (Vector2.Distance(deleteStartPosition, screenPosition) > deletionDragThreshold)
                {
                    deletionTouchActive = false;
                    deleteCandidate = null;
                }
            }
        }
        else if (phase == TouchPhase.Ended)
        {
            if (deletionTouchActive)
            {
                // Confirm deletion if the finger is still over the same delete collider.
                Ray ray = Camera.main.ScreenPointToRay(screenPosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider == deleteCandidate)
                {
                    HandleDeletion(hitInfo.collider.transform.root.gameObject);
                }
                deletionTouchActive = false;
                deleteCandidate = null;
            }
        }
    }

    void HandleDeletion(GameObject obj)
    {
        // Update inventory based on prefab type.
        PlaceableData data = obj.GetComponent<PlaceableData>();
        if (data != null)
        {
            if (data.prefabType == PrefabType.Type1)
            {
                inventoryCount1++;
            }
            else if (data.prefabType == PrefabType.Type2)
            {
                inventoryCount2++;
            }
        }
        Destroy(obj);
    }

    void PlaceObject(Pose hitPose)
    {
        if (currentPrefab == PrefabType.Type1 && inventoryCount1 > 0)
        {
            GameObject obj = Instantiate(prefab1, hitPose.position, hitPose.rotation);
            inventoryCount1--;
            AttachPlaceableData(obj, PrefabType.Type1);
        }
        else if (currentPrefab == PrefabType.Type2 && inventoryCount2 > 0)
        {
            GameObject obj = Instantiate(prefab2, hitPose.position, hitPose.rotation);
            inventoryCount2--;
            AttachPlaceableData(obj, PrefabType.Type2);
        }
        else
        {
            Debug.Log("No inventory available for the selected prefab type.");
        }
    }

    void AttachPlaceableData(GameObject obj, PrefabType type)
    {
        PlaceableData data = obj.GetComponent<PlaceableData>();
        if (data == null)
        {
            data = obj.AddComponent<PlaceableData>();
        }
        data.prefabType = type;
    }

    // Public methods for UI buttons to switch prefab types.
    public void SetPrefabType1()
    {
        currentPrefab = PrefabType.Type1;
    }

    public void SetPrefabType2()
    {
        currentPrefab = PrefabType.Type2;
    }

    public void SwitchPrefab()
    {
        currentPrefab = (currentPrefab == PrefabType.Type1) ? PrefabType.Type2 : PrefabType.Type1;
    }

    // Helper component attached to placed objects to record their prefab type.
    public class PlaceableData : MonoBehaviour
    {
        public PrefabType prefabType;
    }
}
