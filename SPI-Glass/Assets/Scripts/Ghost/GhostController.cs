using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GhostPlaneController : MonoBehaviour
{
    [SerializeField] private GameObject ghostPrefab; // The ghost prefab to be spawned
    [SerializeField] private float planeSizeThreshold = 2.0f; // Minimum size of the plane
    [SerializeField] private float bobbingSpeed = 2.0f; // Speed of bobbing
    [SerializeField] private float bobbingAmount = 0.5f; // Bobbing height
    [SerializeField] private float maxHeightAbovePlane = 5.0f; // Maximum height above the plane
    [SerializeField] private float movementRange = 3.0f; // Range of random movement around the anchor point
    [SerializeField] private float movementDuration = 5.0f; // Duration of random movement
    [SerializeField] private float minPauseDuration = 1.0f; // Minimum pause duration
    [SerializeField] private float maxPauseDuration = 3.0f; // Maximum pause duration
    [SerializeField] private float spawnDelay = 5.0f; // Delay in seconds before spawning the ghost

    private ARPlaneManager planeManager; // ARPlaneManager reference
    private GameObject ghostInstance; // Reference to the ghost prefab
    private Vector3 anchorPosition; // Anchor position on the plane
    private Vector3 basePosition; // Base position for bobbing
    private bool isMoving = true; // Movement state

    void Start()
    {
        // Get the ARPlaneManager component
        planeManager = GetComponent<ARPlaneManager>();

        if (planeManager == null)
        {
            Debug.LogError("ARPlaneManager not found! Ensure it's attached to the same GameObject.");
            enabled = false;
            return;
        }

        // Try to find a suitable plane and start the delay coroutine
        FindAnchorAndSpawn();
    }

    void Update()
    {
        if (ghostInstance != null)
        {
            HandleBobbing();
        }
    }

    private void FindAnchorAndSpawn()
    {
        foreach (ARPlane plane in planeManager.trackables)
        {
            Debug.Log($"Detected plane with size {plane.size.x * plane.size.y}");

            if (plane.alignment == PlaneAlignment.HorizontalUp || plane.alignment == PlaneAlignment.Vertical)
            {
                if (plane.size.x * plane.size.y >= planeSizeThreshold) // Check if the plane is large enough
                {
                    anchorPosition = plane.center; // Use the plane's center as the anchor point
                    Debug.Log($"Anchor found at position: {anchorPosition}. Waiting {spawnDelay} seconds to spawn ghost.");
                    StartCoroutine(SpawnGhostWithDelay());
                    return; // Exit after finding a suitable plane
                }
            }
        }

        Debug.LogWarning("No suitable plane found for spawning the ghost.");
    }

    private IEnumerator SpawnGhostWithDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(spawnDelay);

        if (ghostPrefab == null)
        {
            Debug.LogError("Ghost prefab is not assigned in the Inspector!");
            yield break;
        }

        // Spawn the ghost at the anchor position
        Vector3 spawnPosition = anchorPosition + Vector3.up * maxHeightAbovePlane;
        ghostInstance = Instantiate(ghostPrefab, spawnPosition, Quaternion.identity);

        if (ghostInstance != null)
        {
            // Set the base position for bobbing
            basePosition = spawnPosition;

            // Start random movement behavior
            StartCoroutine(HandleGhostMovement());
            Debug.Log($"Ghost spawned at position: {spawnPosition} after a {spawnDelay}-second delay.");
        }
        else
        {
            Debug.LogError("Failed to instantiate the ghost prefab.");
        }
    }

    private void HandleBobbing()
    {
        if (ghostInstance != null)
        {
            // Apply a bobbing offset
            Vector3 bobbingOffset = Vector3.up * Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
            ghostInstance.transform.position = basePosition + bobbingOffset;
        }
    }

    private IEnumerator HandleGhostMovement()
    {
        while (true)
        {
            if (isMoving)
            {
                // Generate a new random target position near the anchor position
                Vector3 randomOffset = new Vector3(
                    Random.Range(-movementRange, movementRange),
                    Random.Range(0.0f, maxHeightAbovePlane),
                    Random.Range(-movementRange, movementRange)
                );

                Vector3 targetPosition = anchorPosition + randomOffset;

                // Move the ghost to the target position
                float elapsedTime = 0;
                Vector3 startPosition = ghostInstance.transform.position;

                while (elapsedTime < movementDuration)
                {
                    ghostInstance.transform.position = Vector3.Lerp(
                        startPosition,
                        targetPosition,
                        elapsedTime / movementDuration
                    );
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                // Update the base position for bobbing to the final position
                basePosition = ghostInstance.transform.position;
            }

            // Pause movement for a random duration
            isMoving = false;
            yield return new WaitForSeconds(Random.Range(minPauseDuration, maxPauseDuration));

            // Resume movement
            isMoving = true;
        }
    }
}
