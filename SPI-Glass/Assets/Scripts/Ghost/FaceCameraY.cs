using UnityEngine;

public class FaceCameraY : MonoBehaviour
{
    private Transform cameraTransform;

    void Start()
    {
        // Get the main camera's transform
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogError("No main camera found! Ensure a camera is tagged as 'MainCamera'.");
        }
    }

    void Update()
    {
        if (cameraTransform == null) return;

        // Get the direction from this object to the camera
        Vector3 directionToCamera = cameraTransform.position - transform.position;

        // Zero out the Y component to keep the object upright
        directionToCamera.y = 0;

        // Ensure the direction is not zero (prevents issues)
        if (directionToCamera.sqrMagnitude > 0.001f)
        {
            // Rotate towards the camera while keeping the Y-axis fixed
            transform.rotation = Quaternion.LookRotation(directionToCamera);
        }
    }
}
