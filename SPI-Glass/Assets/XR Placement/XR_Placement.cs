using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class XR_Placement : MonoBehaviour
{
    [SerializeField] private GameObject knightPrefab;
    private GameObject spawnedKnight;
    private ARRaycastManager raycastManager;
    private Camera arCamera;

    [SerializeField] private float minPlaneSize = 1.0f;  // Minimum required plane size
    [SerializeField] private float maxPlacementDistance = 3.0f;  // Max distance from the camera for placing prefab
    [SerializeField] private float walkSpeed = 1.0f;     // Speed at which the knight walks toward the camera
    private bool isWalkingTowardsCamera = false;
    private ARPlane currentPlane; // To store the plane on which the knight was placed

    private Coroutine walkCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        arCamera = Camera.main;  // Get the AR camera
    }

    void TryPlaceKnight()
    {
        List<ARRaycastHit> hits = new();

        if (raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinBounds))
        {
            // Go through each detected hit and check conditions
            foreach (ARRaycastHit hit in hits)
            {
                Pose pose = hit.pose;

                // Check if the detected plane is big enough
                if (hit.trackable is ARPlane plane && plane.size.x >= minPlaneSize && plane.size.y >= minPlaneSize)
                {
                    // Check if the plane is at the required distance from the camera
                    float distanceToCamera = Vector3.Distance(arCamera.transform.position, pose.position);
                    if (distanceToCamera <= maxPlacementDistance)
                    {
                        PlaceKnightAtCenterOfPlane(plane);
                        break;
                    }
                }
            }
        }
    }

    void PlaceKnightAtCenterOfPlane(ARPlane plane)
    {
        // Get the center of the detected plane
        Vector3 centerPosition = plane.center;

        // If the knight has not been placed yet, instantiate it
        if (spawnedKnight == null)
        {
            spawnedKnight = Instantiate(knightPrefab, centerPosition, Quaternion.identity);
            currentPlane = plane; // Store the plane reference
        }
    }

    // This function makes the spawned knight rotate towards the AR camera smoothly
    void RotateTowardsCamera()
    {
        if (spawnedKnight != null)
        {
            Vector3 directionToCamera = arCamera.transform.position - spawnedKnight.transform.position;
            directionToCamera.y = 0;  // Keep the knight upright

            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
            spawnedKnight.transform.rotation = Quaternion.Slerp(spawnedKnight.transform.rotation, targetRotation, Time.deltaTime * 2f);
        }
    }

    // This function makes the knight walk closer to the camera
    void WalkTowardsCamera()
    {
        if (spawnedKnight != null && isWalkingTowardsCamera)
        {
            Vector3 direction = (arCamera.transform.position - spawnedKnight.transform.position).normalized;
            direction.y = 0;  // Keep the knight on the ground

            // Move the knight
            Vector3 newPosition = spawnedKnight.transform.position + direction * walkSpeed * Time.deltaTime;

            // Check if the new position is still within the bounds of the plane
            if (currentPlane != null)
            {
                Vector2 planeSize = currentPlane.size;
                Vector3 planeCenter = currentPlane.center;
                Vector3 planeNormal = currentPlane.transform.up;

                // Project the new position onto the plane
                Vector3 projectedPosition = newPosition;
                projectedPosition.y = planeCenter.y;

                // Calculate distances from the center of the plane
                Vector2 distanceFromCenter = new Vector2(
                    Mathf.Abs(projectedPosition.x - planeCenter.x),
                    Mathf.Abs(projectedPosition.z - planeCenter.z)
                );

                // Check if the knight is within the plane bounds
                if (distanceFromCenter.x > planeSize.x / 2 || distanceFromCenter.y > planeSize.y / 2)
                {
                    // If outside the plane bounds, stop moving
                    isWalkingTowardsCamera = false;
                    return;
                }
            }

            // Update position
            spawnedKnight.transform.position = newPosition;
        }
    }

    IEnumerator StopWalkingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isWalkingTowardsCamera = false;
    }

    // Update is called once per frame
    void Update()
    {
        // If no knight is placed, try placing it
        if (spawnedKnight == null)
        {
            TryPlaceKnight();
        }

        // Rotate the knight to face the camera smoothly
        RotateTowardsCamera();

        // Move the knight if it's walking toward the camera
        WalkTowardsCamera();

        // Handle tap or click to make the knight walk
        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            if (spawnedKnight != null)
            {
                isWalkingTowardsCamera = true;

                // Start coroutine to stop walking after 5 seconds
                if (walkCoroutine != null)
                {
                    StopCoroutine(walkCoroutine);
                }
                walkCoroutine = StartCoroutine(StopWalkingAfterDelay(5.0f));
            }
        }
    }
}
