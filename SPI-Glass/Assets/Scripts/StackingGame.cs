using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // For scene management

public class StackingGame : MonoBehaviour
{
    [Header("Puzzle Settings")]
    public GameObject[] puzzlePieces = new GameObject[5]; // The 5 pieces
    public GameObject finalModel;                          // Final model to display when complete
    public float tolerance = 0.1f;                         // Tolerance for centering
    public Vector2 speedRange = new Vector2(1f, 3f);       // Min and Max speed
    public float movementDistance = 2f;                    // Max distance from center point

    [Header("Fade In Settings")]
    public float fadeDuration = 2f;                        // Duration for fading in the final model

    [Header("Piece Placement Settings")]
    public float interpolationDuration = 0.5f;             // Duration for interpolating the piece to original position
    public float delayBeforeNextPiece = 1f;                // Delay before spawning the next piece after successful placement

    [Header("Scene Transition Settings")]
    public float delayBeforeSceneChange = 3f;              // Time to wait before starting the transition animation
    public string nextSceneName = "Scene3";                // Name of the scene to load

    [Header("Transition Animation")]
    [SerializeField] private Animator transition;                        // Animator for the transition
    public string transitionTriggerName = "Start";         // Name of the trigger parameter in the Animator
    public float transitionDuration = 3f;                  // Duration of the transition animation

    private int currentPieceIndex = 0;                     // Tracks which piece is active
    private bool isPuzzleComplete = false;                 // Tracks if puzzle is complete
    private Coroutine currentCoroutine = null;

    // Store original positions and rotations of pieces
    private Vector3[] originalPositions;
    private Quaternion[] originalRotations;

    // Store movement direction vectors and speeds for each piece
    private Vector3[] movementDirections;
    private float[] movementSpeeds;

    // Store time offsets for each piece to control starting positions
    private float[] timeOffsets;

    private void Start()
    {
        // Hide all pieces and the final model initially
        foreach (var piece in puzzlePieces)
        {
            piece.SetActive(false);
        }
        finalModel.SetActive(false);

        int numPieces = puzzlePieces.Length;
        originalPositions = new Vector3[numPieces];
        originalRotations = new Quaternion[numPieces];
        movementDirections = new Vector3[numPieces];
        movementSpeeds = new float[numPieces];
        timeOffsets = new float[numPieces];

        for (int i = 0; i < numPieces; i++)
        {
            originalPositions[i] = puzzlePieces[i].transform.localPosition;
            originalRotations[i] = puzzlePieces[i].transform.localRotation;
        }

        // Start the puzzle with the first piece
        StartPieceMovement(currentPieceIndex);
    }

    private void StartPieceMovement(int index)
    {
        if (index >= puzzlePieces.Length)
        {
            CompletePuzzle();
            return;
        }

        GameObject piece = puzzlePieces[index];
        piece.SetActive(true);

        // Generate and store random speed and direction for the piece
        float speed = Random.Range(speedRange.x, speedRange.y);
        movementSpeeds[index] = speed;

        float angle = Random.Range(0f, 360f);
        Vector3 directionVector = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
        movementDirections[index] = directionVector;

        // Generate a random time offset to start at a random position along the path
        float timeOffset = Random.Range(0f, (2f * movementDistance) / speed);
        timeOffsets[index] = timeOffset;

        // Reset the piece's position and rotation
        piece.transform.localPosition = originalPositions[index];
        piece.transform.localRotation = originalRotations[index];

        // Start the movement coroutine
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(MovePiece(piece, index));
    }

    private IEnumerator MovePiece(GameObject piece, int index)
    {
        Vector3 origin = originalPositions[index];
        Vector3 directionVector = movementDirections[index];
        float speed = movementSpeeds[index];
        float timeOffset = timeOffsets[index];

        while (!isPuzzleComplete)
        {
            // Update elapsed time
            float elapsedTime = Time.time + timeOffset;

            // Calculate position along the movement line using Mathf.PingPong
            float distanceAlongPath = Mathf.PingPong(elapsedTime * speed, 2f * movementDistance) - movementDistance;

            // Update the piece's position
            piece.transform.localPosition = origin + directionVector * distanceAlongPath;

            // Check for input
            if (IsInputReceived())
            {
                // Calculate the distance from the origin along the direction vector
                float distanceFromOrigin = Vector3.Dot(piece.transform.localPosition - origin, directionVector);

                bool isCentered = Mathf.Abs(distanceFromOrigin) <= tolerance;

                if (isCentered)
                {
                    // Interpolate the piece to its original position and rotation
                    yield return StartCoroutine(InterpolatePieceToOriginal(piece, index));

                    // Wait before spawning the next piece
                    yield return new WaitForSeconds(delayBeforeNextPiece);

                    // Proceed to the next piece
                    currentPieceIndex++;
                    StartPieceMovement(currentPieceIndex);
                    yield break;
                }
                else
                {
                    // Drop the piece and respawn
                    Rigidbody rb = piece.GetComponent<Rigidbody>();
                    rb.isKinematic = false;

                    yield return new WaitForSeconds(2f);

                    // Reset the piece
                    rb.isKinematic = true;

                    // Generate a new random time offset for starting position
                    timeOffset = Random.Range(0f, (2f * movementDistance) / speed);
                    timeOffsets[index] = timeOffset;

                    // Reset position and rotation
                    piece.transform.localPosition = origin;
                    piece.transform.localRotation = originalRotations[index];
                }
            }

            yield return null;
        }
    }

    private IEnumerator InterpolatePieceToOriginal(GameObject piece, int index)
    {
        Vector3 startPosition = piece.transform.localPosition;
        Quaternion startRotation = piece.transform.localRotation;

        Vector3 endPosition = originalPositions[index];
        Quaternion endRotation = originalRotations[index];

        float elapsedTime = 0f;

        while (elapsedTime < interpolationDuration)
        {
            piece.transform.localPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / interpolationDuration);
            piece.transform.localRotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / interpolationDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the piece is exactly at the original position and rotation
        piece.transform.localPosition = endPosition;
        piece.transform.localRotation = endRotation;
    }

    private bool IsInputReceived()
    {
        // Check for mouse input
        if (Input.GetMouseButtonDown(0))
        {
            return true;
        }

        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                return true;
            }
        }

        return false;
    }

    private void CompletePuzzle()
    {
        isPuzzleComplete = true;

        // Hide all pieces
        foreach (var piece in puzzlePieces)
        {
            piece.SetActive(false);
        }

        // Show and fade in the final model
        finalModel.SetActive(true);
        StartCoroutine(FadeInFinalModel());
    }

    private IEnumerator FadeInFinalModel()
    {
        Renderer[] renderers = finalModel.GetComponentsInChildren<Renderer>();
        // Store original colors
        Dictionary<Material, Color> originalColors = new Dictionary<Material, Color>();

        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                if (!originalColors.ContainsKey(mat))
                {
                    originalColors[mat] = mat.color;
                    mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 0);
                }
            }
        }

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = elapsedTime / fadeDuration;
            foreach (Renderer rend in renderers)
            {
                foreach (Material mat in rend.materials)
                {
                    Color originalColor = originalColors[mat];
                    mat.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final alpha is set to 1
        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                Color originalColor = originalColors[mat];
                mat.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1);
            }
        }

        // Start coroutine to change scene after delay
        StartCoroutine(ChangeSceneAfterDelay());
    }

    private IEnumerator ChangeSceneAfterDelay()
    {
        // Wait for the specified delay before starting the transition
        yield return new WaitForSeconds(delayBeforeSceneChange);

        // Start the transition animation
        if (transition != null)
        {
            transition.SetTrigger(transitionTriggerName);
        }

        // Wait for the transition animation to complete
        yield return new WaitForSeconds(transitionDuration);

        // Load the next scene
        SceneManager.LoadScene(nextSceneName);
    }
}
