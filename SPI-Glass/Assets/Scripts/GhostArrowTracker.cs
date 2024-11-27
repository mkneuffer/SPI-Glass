using UnityEngine;

public class GhostArrowTracker : MonoBehaviour
{
    [SerializeField] private RectTransform arrowUI; // Assign the arrow UI RectTransform
    [SerializeField] private float sideActivationAngle = 30.0f; // Activation angle for sides
    [SerializeField] private float verticalActivationAngle = 45.0f; // Activation angle for top/bottom
    [SerializeField] private float screenEdgeBuffer = 200.0f; // Buffer from screen edges (in pixels)
    [SerializeField] private Transform customTarget; // Optional: A custom target point on the prefab
    [SerializeField] private Vector3 targetOffset = Vector3.zero; // Optional: Offset from prefab origin

    private Camera mainCamera;
    private Transform ghostTransform; // Reference to the spawned ghost's Transform
    private Canvas canvas; // Reference to the canvas containing the arrow

    void Start()
    {
        mainCamera = Camera.main;

        if (arrowUI != null)
        {
            arrowUI.gameObject.SetActive(false); // Initially hide the arrow
        }

        // Subscribe to the OnGhostSpawned event
        XR_Placement.OnGhostSpawned += AssignGhostTransform;

        // Find the canvas component
        canvas = arrowUI.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Arrow UI must be a child of a Canvas.");
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from the OnGhostSpawned event
        XR_Placement.OnGhostSpawned -= AssignGhostTransform;
    }

    void Update()
    {
        if (ghostTransform == null || arrowUI == null || mainCamera == null)
        {
            return; // Exit if no ghost or arrow is present
        }

        // Determine the target point (custom target or offset)
        Vector3 targetPoint = customTarget != null ? customTarget.position : ghostTransform.position + targetOffset;

        // Direction from the camera to the target point
        Vector3 directionToTarget = targetPoint - mainCamera.transform.position;

        // Horizontal angle: Angle between the forward vector and the target's horizontal direction
        float horizontalAngle = Vector3.Angle(new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z),
                                              new Vector3(directionToTarget.x, 0, directionToTarget.z));

        // Vertical angle: Angle between the forward vector's Y component and the target's Y direction
        float verticalAngle = Mathf.Abs(Vector3.Angle(Vector3.up, directionToTarget) - 90.0f);

        // Determine visibility based on activation angles
        bool isOutsideHorizontalAngle = horizontalAngle > sideActivationAngle;
        bool isOutsideVerticalAngle = verticalAngle > verticalActivationAngle;

        // If the ghost is within both activation ranges, hide the arrow
        if (!isOutsideHorizontalAngle && !isOutsideVerticalAngle)
        {
            arrowUI.gameObject.SetActive(false); // Hide the arrow
            return;
        }

        // Project the target point's world position into screen space
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(targetPoint);

        // Handle the case where the target is behind the camera
        if (screenPoint.z < 0)
        {
            // Flip the arrow position to simulate pointing in the correct direction
            screenPoint.x = Screen.width - screenPoint.x;
            screenPoint.y = Screen.height - screenPoint.y;
            screenPoint.z = Mathf.Abs(screenPoint.z); // Ensure positive Z for further calculations
        }

        // Restrict the arrow to the screen bounds with buffer
        screenPoint.x = Mathf.Clamp(screenPoint.x, screenEdgeBuffer, Screen.width - screenEdgeBuffer);
        screenPoint.y = Mathf.Clamp(screenPoint.y, Screen.height / 4 + screenEdgeBuffer, Screen.height - screenEdgeBuffer);

        // Convert screen point to canvas space
        Vector2 canvasPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPoint,
            canvas.worldCamera,
            out canvasPosition
        );

        // Update arrow position
        arrowUI.anchoredPosition = canvasPosition;

        // Rotate the arrow to face the closest edge of the screen toward the target
        Vector2 directionToEdge = (screenPoint - new Vector3(Screen.width / 2, Screen.height / 2)).normalized;
        float rotationAngle = Mathf.Atan2(directionToEdge.y, directionToEdge.x) * Mathf.Rad2Deg;
        arrowUI.localRotation = Quaternion.Euler(0, 0, rotationAngle);

        // Show the arrow if hidden
        if (!arrowUI.gameObject.activeSelf)
        {
            arrowUI.gameObject.SetActive(true);
        }
    }

    private void AssignGhostTransform(Transform ghost)
    {
        ghostTransform = ghost;
        Debug.Log("Ghost Transform assigned to Arrow Tracker.");
    }
}
