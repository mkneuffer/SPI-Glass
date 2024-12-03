using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CirclePuzzle : MonoBehaviour
{
    [Header("Circle Objects")]
    public Transform[] circles;  // Assign your circle GameObjects here

    [Header("Scene Transition")]
    public string nextSceneName;  // Name of the scene to load after puzzle completion

    private bool[] isRotating;    // Flags to indicate if a circle is currently rotating
    private bool puzzleComplete = false;

    // Input handling variables
    private Transform selectedCircle = null;
    private Vector2 previousInputPosition;
    private bool isDragging = false;
    private const float dragThreshold = 5f;  // Minimum distance to consider as a drag

    // Default rotation values
    private Vector3 defaultRotation = new Vector3(0f, -180f, -15f);

    void Start()
    {
        if (circles == null || circles.Length == 0)
        {
            Debug.LogError("No circles have been assigned in the inspector.");
            return;
        }

        int circleCount = circles.Length;
        isRotating = new bool[circleCount];

        for (int i = 0; i < circleCount; i++)
        {
            isRotating[i] = false;

            // Set each circle to the default rotation
            circles[i].localEulerAngles = defaultRotation;

            // Apply a random rotation around the local Y-axis, excluding zero degrees
            int randomStep = Random.Range(1, 12);  // 1 to 11 inclusive
            float randomRotation = randomStep * 30f;

            // Ensure the random rotation does not result in the default rotation
            if (randomRotation % 360f == 0f)
            {
                randomRotation += 30f;
            }

            // Apply the random rotation
            circles[i].localRotation = circles[i].localRotation * Quaternion.Euler(0f, randomRotation, 0f);
        }
    }

    void Update()
    {
        if (puzzleComplete)
            return;

        HandleInput();

        // Check if all circles are aligned (rotation matches defaultRotation within tolerance)
        if (AllCirclesAligned())
        {
            puzzleComplete = true;
            StartCoroutine(CompletePuzzle());
        }
    }

    void HandleInput()
    {
        // Handle touch input
        if (Input.touchSupported && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = touch.position;

            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            RaycastHit hit;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (IsCircle(hit.transform))
                        {
                            selectedCircle = hit.transform;
                            previousInputPosition = touchPosition;
                            isDragging = false;
                        }
                    }
                    break;

                case TouchPhase.Moved:
                    if (selectedCircle != null)
                    {
                        if (!isDragging && Vector2.Distance(previousInputPosition, touchPosition) > dragThreshold)
                        {
                            isDragging = true;
                        }

                        if (isDragging)
                        {
                            RotateCircle(selectedCircle, previousInputPosition, touchPosition);
                            previousInputPosition = touchPosition;
                        }
                    }
                    break;

                case TouchPhase.Ended:
                    if (selectedCircle != null)
                    {
                        if (!isDragging)
                        {
                            // Tap detected, rotate 30 degrees counterclockwise
                            StartCoroutine(RotateToAngle(selectedCircle, -30f));
                        }
                        else
                        {
                            // Dragging ended, snap to nearest 30 degrees
                            SnapCircle(selectedCircle);
                        }
                        selectedCircle = null;
                    }
                    break;
            }
        }
        // Handle mouse input
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePosition = Input.mousePosition;
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (IsCircle(hit.transform))
                    {
                        selectedCircle = hit.transform;
                        previousInputPosition = mousePosition;
                        isDragging = false;
                    }
                }
            }
            else if (Input.GetMouseButton(0))
            {
                if (selectedCircle != null)
                {
                    Vector2 mousePosition = Input.mousePosition;
                    if (!isDragging && Vector2.Distance(previousInputPosition, mousePosition) > dragThreshold)
                    {
                        isDragging = true;
                    }

                    if (isDragging)
                    {
                        RotateCircle(selectedCircle, previousInputPosition, mousePosition);
                        previousInputPosition = mousePosition;
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (selectedCircle != null)
                {
                    if (!isDragging)
                    {
                        // Click detected, rotate 30 degrees counterclockwise
                        StartCoroutine(RotateToAngle(selectedCircle, -30f));
                    }
                    else
                    {
                        // Dragging ended, snap to nearest 30 degrees
                        SnapCircle(selectedCircle);
                    }
                    selectedCircle = null;
                }
            }
        }
    }

    void RotateCircle(Transform circle, Vector2 previousPosition, Vector2 currentPosition)
    {
        // Calculate the rotation based on the movement direction
        Vector2 delta = currentPosition - previousPosition;

        // Project the movement onto the screen space around the circle
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(circle.position);
        Vector2 circleCenter = new Vector2(screenPoint.x, screenPoint.y);

        Vector2 prevDirection = previousPosition - circleCenter;
        Vector2 currDirection = currentPosition - circleCenter;

        float angle = Vector2.SignedAngle(prevDirection, currDirection);

        // Rotate around the circle's local Y-axis
        circle.Rotate(Vector3.up, -angle, Space.Self); // Negative to match drag direction
    }

    void SnapCircle(Transform circle)
    {
        // Get the current rotation around the local Y-axis
        float currentRotation = circle.localEulerAngles.y;
        float snappedRotation = Mathf.Round(currentRotation / 30f) * 30f;
        float deltaRotation = Mathf.DeltaAngle(currentRotation, snappedRotation);
        StartCoroutine(RotateToAngle(circle, deltaRotation));
    }

    IEnumerator RotateToAngle(Transform circle, float angle)
    {
        int index = GetCircleIndex(circle);
        if (isRotating[index])
            yield break;  // Prevent multiple rotations on the same circle

        isRotating[index] = true;

        float duration = 0.25f;  // Rotation duration
        float elapsed = 0f;
        Quaternion initialRotation = circle.localRotation;
        Quaternion targetRotation = initialRotation * Quaternion.Euler(0f, angle, 0f);

        while (elapsed < duration)
        {
            circle.localRotation = Quaternion.Slerp(initialRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        circle.localRotation = targetRotation;
        isRotating[index] = false;
    }

    bool AllCirclesAligned()
    {
        foreach (Transform circle in circles)
        {
            Vector3 currentRotation = circle.localEulerAngles;

            // Adjust angles to be between -180 and 180 degrees
            float currentY = Mathf.DeltaAngle(0f, currentRotation.y);
            float currentZ = Mathf.DeltaAngle(0f, currentRotation.z);

            // Compare with default rotation
            float targetY = defaultRotation.y;
            float targetZ = defaultRotation.z;

            float deltaY = Mathf.Abs(Mathf.DeltaAngle(currentY, targetY));
            float deltaZ = Mathf.Abs(Mathf.DeltaAngle(currentZ, targetZ));

            // Check if rotations are within a small tolerance (e.g., 1 degree)
            if (deltaY > 1f || deltaZ > 1f)
                return false;
        }
        return true;
    }

    IEnumerator CompletePuzzle()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(nextSceneName);
    }

    int GetCircleIndex(Transform circle)
    {
        for (int i = 0; i < circles.Length; i++)
        {
            if (circles[i] == circle)
                return i;
        }
        return -1;
    }

    bool IsCircle(Transform obj)
    {
        foreach (Transform circle in circles)
        {
            if (obj == circle)
                return true;
        }
        return false;
    }
}
