using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class XR_Placement : MonoBehaviour
{
    [SerializeField] private GameObject prefab; // Prefab to be spawned
    [SerializeField] private float minimumSpawnDistance = 1.0f; // Minimum distance to camera for spawning
    [SerializeField] private float maximumSpawnDistance = 10.0f; // Maximum distance to camera for spawning
    private ARRaycastManager raycastManager; // Reference to the ARRaycastManager
    private ARPlaneManager planeManager; // Reference to the ARPlaneManager
    private bool hasSpawned = false; // Flag to ensure only one spawn
    private GameObject spawnedObject; // Reference to the spawned prefab

    // Start is called before the first frame update
    void Start()
    {
        // Get the ARRaycastManager component from the same GameObject
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();

        if (raycastManager == null || planeManager == null)
        {
            Debug.LogError("ARRaycastManager or ARPlaneManager not found! Ensure they are attached to the same GameObject.");
            enabled = false;
            return;
        }
    }

    // Public function to spawn the prefab manually
    public void SpawnGhost()
    {
        // Exit if we've already spawned the prefab
        if (hasSpawned)
        {
            Debug.LogWarning("The ghost has already been spawned.");
            return;
        }

        // Iterate through all detected planes
        foreach (ARPlane plane in planeManager.trackables)
        {
            // Check if the plane is horizontal
            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                Vector3 planePosition = plane.center;

                // Calculate the distance between the camera and the plane
                float distanceToCamera = Vector3.Distance(Camera.main.transform.position, planePosition);

                // Check if the plane is within the distance range
                if (distanceToCamera >= minimumSpawnDistance && distanceToCamera <= maximumSpawnDistance)
                {
                    Quaternion lookAtCameraRotation = CalculateLookAtCameraRotation(planePosition); // Get rotation to face the camera
                    SpawnPrefab(planePosition, lookAtCameraRotation); // Spawn prefab
                    hasSpawned = true; // Mark as spawned
                    Debug.Log("Ghost spawned successfully!");
                    return; // Exit after spawning
                }
            }
        }

        Debug.LogWarning("No suitable plane detected to spawn the ghost within the specified distance range.");
    }

    // Calculate rotation to make the prefab face the camera
    private Quaternion CalculateLookAtCameraRotation(Vector3 spawnPosition)
    {
        Vector3 directionToCamera = Camera.main.transform.position - spawnPosition;
        directionToCamera.y = 0; // Keep the rotation on the horizontal plane
        return Quaternion.LookRotation(directionToCamera);
    }

    // Spawn the prefab at the given position and rotation
    private void SpawnPrefab(Vector3 position, Quaternion rotation)
    {
        spawnedObject = Instantiate(prefab, position, rotation); // Instantiate the prefab with the calculated rotation
        Debug.Log($"Prefab spawned at position: {position} and is facing the camera.");
    }

    // Public function to delete the prefab from the scene
    public void DeleteGhost()
    {
        // Check if a prefab has been spawned
        if (spawnedObject != null)
        {
            Destroy(spawnedObject); // Destroy the spawned prefab
            hasSpawned = false; // Reset the spawn flag
            spawnedObject = null; // Clear the reference to the prefab
            Debug.Log("Ghost deleted successfully!");
        }
        else
        {
            Debug.LogWarning("No ghost found to delete.");
        }
    }
}
