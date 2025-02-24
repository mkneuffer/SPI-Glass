using UnityEngine;
using System.Collections;

public class CirclePuzzle : MonoBehaviour
{
    [Header("Circle Objects")]
    public Transform[] circles;  // Assign your circle GameObjects here

    [Header("Dialogue")]
    public DialogueManager dialogueManager;  // Reference to the DialogueManager
    public TextAsset nextInkJSON;  // The next dialogue to load after puzzle completion

    private bool[] isRotating;    // Flags to indicate if a circle is currently rotating
    private bool puzzleComplete = false;

    // Input handling variables
    private Transform selectedCircle = null;
    private Vector2 previousInputPosition;
    private bool isDragging = false;
    private const float dragThreshold = 5f;  // Minimum distance to consider as a drag

    // Store the initial rotations for alignment checks
    private Quaternion[] initialRotations;

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager not found in the scene.");
            return;
        }

        if (circles == null || circles.Length == 0)
        {
            Debug.LogError("No circles have been assigned in the inspector.");
            return;
        }

        int circleCount = circles.Length;
        isRotating = new bool[circleCount];
        initialRotations = new Quaternion[circleCount];

        for (int i = 0; i < circleCount; i++)
        {
            isRotating[i] = false;

            // Store the initial rotation for alignment checks
            initialRotations[i] = circles[i].localRotation;

            // Set a random initial rotation (multiple of 30 degrees), excluding zero degrees
            int randomStep = Random.Range(1, 12);  // 1 to 11 inclusive, excludes 0
            float randomRotation = randomStep * 30f;

            // Apply random rotation around the circle's local Z-axis
            circles[i].localRotation *= Quaternion.Euler(0f, 0f, randomRotation);
        }
    }

    void Update()
    {
        if (puzzleComplete)
            return;

        HandleInput();

        // Check if all circles are aligned
        if (AllCirclesAligned())
        {
            puzzleComplete = true;
            StartCoroutine(CompletePuzzle());
        }
    }

    void HandleInput()
    {
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
                            StartCoroutine(RotateByAngle(selectedCircle, -30f));
                        }
                        else
                        {
                            SnapCircle(selectedCircle);
                        }
                        selectedCircle = null;
                    }
                    break;
            }
        }
        else if (Input.GetMouseButtonDown(0))
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
                    StartCoroutine(RotateByAngle(selectedCircle, -30f));
                }
                else
                {
                    SnapCircle(selectedCircle);
                }
                selectedCircle = null;
            }
        }
    }

    void RotateCircle(Transform circle, Vector2 previousPosition, Vector2 currentPosition)
    {
        Vector2 delta = currentPosition - previousPosition;

        Vector3 screenPoint = Camera.main.WorldToScreenPoint(circle.position);
        Vector2 circleCenter = new Vector2(screenPoint.x, screenPoint.y);

        Vector2 prevDirection = previousPosition - circleCenter;
        Vector2 currDirection = currentPosition - circleCenter;

        float angle = Vector2.SignedAngle(prevDirection, currDirection);

        circle.Rotate(Vector3.forward, -angle, Space.Self);
    }

    void SnapCircle(Transform circle)
    {
        float currentRotation = circle.localEulerAngles.z;
        float snappedRotation = Mathf.Round(currentRotation / 30f) * 30f;
        float deltaRotation = Mathf.DeltaAngle(currentRotation, snappedRotation);

        StartCoroutine(RotateByAngle(circle, deltaRotation));
    }

    IEnumerator RotateByAngle(Transform circle, float angle)
    {
        int index = GetCircleIndex(circle);
        if (isRotating[index])
            yield break;

        isRotating[index] = true;

        float duration = 0.25f;
        float elapsed = 0f;

        Quaternion initialRotation = circle.localRotation;
        Quaternion targetRotation = initialRotation * Quaternion.Euler(0f, 0f, angle);

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
        for (int i = 0; i < circles.Length; i++)
        {
            Quaternion currentRotation = circles[i].localRotation;
            Quaternion deltaRotation = Quaternion.Inverse(initialRotations[i]) * currentRotation;

            float angle = Quaternion.Angle(Quaternion.identity, deltaRotation);

            if (Mathf.Abs(angle) > 1f)
                return false;
        }
        return true;
    }

    IEnumerator CompletePuzzle()
    {
        yield return new WaitForSeconds(0.5f);  // Optional delay for user feedback

        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
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
