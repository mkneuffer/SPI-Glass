using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GhostPlaceController : MonoBehaviour
{
    [SerializeField] private GameObject ghostPrefab; // The ghost prefab to be spawned
    [SerializeField] private float minimumSpawnDistance = 1.0f; // Minimum distance to camera for spawning
    [SerializeField] private float maximumSpawnDistance = 10.0f; // Maximum distance to camera for spawning
    [SerializeField] private float bobbingSpeed = 2.0f; // Speed of bobbing
    [SerializeField] private float bobbingAmount = 0.5f; // Bobbing height

    private ARRaycastManager raycastManager; // Reference to the ARRaycastManager
    private ARPlaneManager planeManager; // Reference to the ARPlaneManager
    private GameObject ghostInstance; // The instantiated ghost
    private Vector3 basePosition; // Base position for bobbing
    private bool isGhostVisible = false; // Tracks if the ghost is visible

    public static System.Action<Transform> OnGhostSpawned;

    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();

        if (raycastManager == null || planeManager == null)
        {
            Debug.LogError("ARRaycastManager or ARPlaneManager not found! Ensure they are attached to the same GameObject.");
            enabled = false;
            return;
        }

        // Instantiate the ghost but keep it inactive
        ghostInstance = Instantiate(ghostPrefab);
        ghostInstance.SetActive(false); // Keep it invisible and inactive initially
    }

    void Update()
    {
        if (!isGhostVisible)
        {
            TryPlaceGhost();
        }

        if (ghostInstance != null)
        {
            HandleBobbing();
        }
    }

    private void TryPlaceGhost()
    {
        float furthestValidDistance = minimumSpawnDistance;
        Vector3 furthestValidPosition = Vector3.zero;
        ARPlane furthestValidPlane = null;

        foreach (ARPlane plane in planeManager.trackables)
        {
            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                Vector3 planePosition = plane.center;

                float distanceToCamera = Vector3.Distance(Camera.main.transform.position, planePosition);

                if (distanceToCamera >= minimumSpawnDistance && distanceToCamera <= maximumSpawnDistance)
                {
                    if (distanceToCamera > furthestValidDistance)
                    {
                        furthestValidDistance = distanceToCamera;
                        furthestValidPosition = planePosition;
                        furthestValidPlane = plane;
                    }
                }
            }
        }

        if (furthestValidPlane != null)
        {
            Vector3 spawnPosition = furthestValidPlane.transform.TransformPoint(furthestValidPlane.center);
            
            // Store the spawn position as the base position for bobbing
            basePosition = spawnPosition;
            
            // Optionally: Add an offset based on the ghost's height/pivot point
            float heightOffset = ghostInstance.GetComponent<Renderer>()?.bounds.extents.y ?? 0.5f;
            spawnPosition.y += heightOffset;
            basePosition.y += heightOffset;

            ghostInstance.transform.position = spawnPosition;

            // Make the ghost face the camera
            Vector3 directionToCamera = (Camera.main.transform.position - spawnPosition).normalized;
            directionToCamera.y = 0; // Keep the rotation on the horizontal plane
            ghostInstance.transform.rotation = Quaternion.LookRotation(directionToCamera);

            // Make the ghost visible and enable animations
            ghostInstance.SetActive(true);
            isGhostVisible = true;

            OnGhostSpawned?.Invoke(ghostInstance.transform);
            Debug.Log($"Ghost spawned at {spawnPosition} on a valid plane at distance {furthestValidDistance}.");
        }
        else
        {
            Debug.LogWarning("No suitable plane found within the specified distance range.");
        }
    }

    private void HandleBobbing()
    {
        if (ghostInstance != null && isGhostVisible) // Only bob when ghost is visible
        {
            Vector3 bobbingOffset = Vector3.up * Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
            ghostInstance.transform.position = basePosition + bobbingOffset;
        }
    }

    public void DeleteGhost()
    {
        if (ghostInstance != null && ghostInstance.activeSelf)
        {
            ghostInstance.SetActive(false); // Make the ghost invisible
            isGhostVisible = false; // Reset visibility flag
            Debug.Log("Ghost has been hidden.");
        }
        else
        {
            Debug.LogWarning("No visible ghost to hide.");
        }
    }
}
