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
    private GameObject instantiatedPrefab; // The instantiated prefab
    private bool isGhostVisible = false; // Tracks visibility state

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

        // Instantiate the prefab but keep it inactive
        instantiatedPrefab = Instantiate(prefab);
        instantiatedPrefab.SetActive(false); // Keep it invisible and inactive initially
    }

    void Update()
    {
        if (!isGhostVisible)
        {
            TryPlacePrefab();
        }
    }

    private void TryPlacePrefab()
    {
        float furthestValidDistance = minimumSpawnDistance; // Start at the minimum distance
        Vector3 furthestValidPosition = Vector3.zero; // Store the position of the furthest valid point
        ARPlane furthestValidPlane = null; // Store the plane with the furthest valid point

        // Loop through all planes detected by the AR Plane Manager
        foreach (ARPlane plane in planeManager.trackables)
        {
            // Ensure the plane is horizontal
            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                Vector3 planePosition = plane.center;

                // Calculate the distance to the camera
                float distanceToCamera = Vector3.Distance(Camera.main.transform.position, planePosition);

                // Check if this plane is within the valid range of distances
                if (distanceToCamera >= minimumSpawnDistance && distanceToCamera <= maximumSpawnDistance)
                {
                    // Check if this is the furthest valid plane so far
                    if (distanceToCamera > furthestValidDistance)
                    {
                        furthestValidDistance = distanceToCamera;
                        furthestValidPosition = planePosition;
                        furthestValidPlane = plane;
                    }
                }
            }
        }

        // If we found a valid plane, update the prefab's position and rotation
        if (furthestValidPlane != null)
        {
            instantiatedPrefab.transform.position = furthestValidPosition;

            // Calculate the direction to face the camera
            Vector3 directionToCamera = (Camera.main.transform.position - furthestValidPosition).normalized;
            directionToCamera.y = 0; // Keep the rotation on the horizontal plane

            // Apply the rotation to face the camera
            instantiatedPrefab.transform.rotation = Quaternion.LookRotation(directionToCamera);

            // Debug log for successful placement
            Debug.Log($"Prefab positioned invisibly at {furthestValidPosition} on a valid plane at distance {furthestValidDistance}.");
        }
        else
        {
            Debug.LogWarning("No suitable plane found within the specified distance range.");
        }
    }

    public void SpawnGhost()
    {
        if (!instantiatedPrefab.activeSelf)
        {
            instantiatedPrefab.SetActive(true);
            isGhostVisible = true;
            EnablePrefabAnimations();

            OnGhostSpawned?.Invoke(instantiatedPrefab.transform);

            Debug.Log("Ghost has been spawned and made visible.");
        }
        else
        {
            Debug.LogWarning("Ghost is already visible.");
        }
    }

    public void DeleteGhost()
    {
        if (instantiatedPrefab != null && instantiatedPrefab.activeSelf)
        {
            instantiatedPrefab.SetActive(false); // Make the prefab invisible
            isGhostVisible = false; // Reset visibility flag
            DisablePrefabAnimations(); // Stop any animations or effects
            Debug.Log("Ghost has been hidden.");
        }
        else
        {
            Debug.LogWarning("No visible ghost to hide.");
        }
    }

    private void EnablePrefabAnimations()
    {
        Animator animator = instantiatedPrefab.GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = true;
        }
    }

    private void DisablePrefabAnimations()
    {
        Animator animator = instantiatedPrefab.GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }
    }
}
