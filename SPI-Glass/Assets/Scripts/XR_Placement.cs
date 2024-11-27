using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class XR_Placement : MonoBehaviour
{
    [SerializeField] private GameObject prefab; // Prefab to be spawned
    [SerializeField] private float minimumSpawnDistance = 1.0f; // Minimum distance to camera for spawning
    private ARRaycastManager raycastManager; // Reference to the ARRaycastManager
    private ARPlaneManager planeManager; // Reference to the ARPlaneManager
    private GameObject instantiatedPrefab; // The instantiated prefab
    private bool isGhostVisible = false; // Tracks visibility state

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
        // Loop through all planes detected by the AR Plane Manager
        foreach (ARPlane plane in planeManager.trackables)
        {
            // Ensure the plane is horizontal and detected
            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                Vector3 planePosition = plane.center;

                // Calculate the distance to the camera
                float distanceToCamera = Vector3.Distance(Camera.main.transform.position, planePosition);

                // Ensure the plane is far enough from the camera
                if (distanceToCamera >= minimumSpawnDistance)
                {
                    // Update the prefab's position to the plane
                    instantiatedPrefab.transform.position = planePosition;

                    // Calculate the direction to face the camera
                    Vector3 directionToCamera = (Camera.main.transform.position - planePosition).normalized;
                    directionToCamera.y = 0; // Keep the rotation on the horizontal plane

                    // Apply the rotation to face the camera (NO inversion)
                    instantiatedPrefab.transform.rotation = Quaternion.LookRotation(directionToCamera);

                    // Debug log for successful placement
                    Debug.Log($"Prefab positioned invisibly at {planePosition} on a valid plane and now correctly facing the camera.");

                    return; // Stop after finding a valid plane
                }
            }
        }

        Debug.LogWarning("No suitable plane found at the required distance.");
    }

    public void SpawnGhost()
    {
        if (!instantiatedPrefab.activeSelf)
        {
            instantiatedPrefab.SetActive(true);
            isGhostVisible = true;
            EnablePrefabAnimations();
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
