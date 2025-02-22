using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinInteraction : MonoBehaviour
{
    [SerializeField] Transform lockpick;
    [SerializeField] float resetSpeed = 5f;
    [SerializeField] Transform resetPosition;
    [SerializeField] Collider successHitbox;
    [SerializeField] Material defaultMaterial; // Default cap material (Red)
    [SerializeField] Material successMaterial; // Cap material when in position (Green)
    [SerializeField] string capObjectName = "PinCap"; // Name of Unity object


    private int totalPins = 5;
    private static int pinsLocked = 0;
    private bool isInteracting = false;
    private bool isLocked = false;
    private bool isResetting = false;
    private Renderer capRenderer;
    private Rigidbody rb;
    private PickMovement pickMovement;

    void Start()
    {
        // Find the cap object
        Transform capTransform = transform.Find(capObjectName);
        if (capTransform != null)
        {
            capRenderer = capTransform.GetComponent<Renderer>();
            capRenderer.material = defaultMaterial;
        }
        else
        {
            Debug.LogError("Pin cap child not found! Check if the name in the Inspector matches.");
        }

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (lockpick != null)
        {
            pickMovement = lockpick.GetComponent<PickMovement>();
        }
        else
        {
            Debug.LogError("Lockpick reference is missing! Assign it in the Inspector.");
        }
    }

    void Update()
    {
        if (isInteracting && pickMovement != null && !isResetting)
        {
            if (pickMovement.velocity.y > 0)
            {
                rb.velocity = new Vector3(0, pickMovement.velocity.y, 0);
            }
            else
            {
                rb.velocity = Vector3.zero; // Pin only moves down when resetting position
            }
        }
        else if (!isLocked && !isResetting)
        {
            // Returns to default position
            rb.velocity = Vector3.zero;
            transform.position = Vector3.MoveTowards(transform.position, resetPosition.position, resetSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lockpick") && !isResetting)
        {
            isInteracting = true;
        }
        else if (other == successHitbox)
        {
            if (!isLocked && !isResetting)
            {
                ChangeCapColor(successMaterial);
                LockPin();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Lockpick"))
        {
            isInteracting = false;
        }
        else if (other == successHitbox)
        {
            ChangeCapColor(defaultMaterial);
            ResetAllPins();
        }
    }

    private void LockPin()
    {
        isLocked = true;
        pinsLocked++;

        if (pinsLocked == totalPins) // Win state for scene transition
        {
            Debug.Log("All pins locked!");
            ChestAnimation.Instance.playChestAnim();
            PinManager.Instance.LoadNextScene(); // PinManager will handle scene change from Lockpick
        }
    }

    private void ResetAllPins()
    {
        Debug.Log("Pin went too high! Resetting all pins.");

        if (lockpick != null)
        {
            PickMovement pickMovement = lockpick.GetComponent<PickMovement>();
            if (pickMovement != null)
            {
                pickMovement.ResetPickPosition();
            }
        }

        PinInteraction[] allPins = FindObjectsOfType<PinInteraction>();
        foreach (PinInteraction pin in allPins)
        {
            pin.StartCoroutine(pin.ResetPinCoroutine());
        }

        // Reset counter
        pinsLocked = 0;
    }

    private IEnumerator ResetPinCoroutine() // Coroutine for pin reset
    {
        isLocked = false;
        isInteracting = false;
        isResetting = true;

        rb.velocity = Vector3.zero;

        while (Vector3.Distance(transform.position, resetPosition.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, resetPosition.position, resetSpeed * Time.deltaTime);
            yield return null;
        }

        // Checks if pin is fully reset
        transform.position = resetPosition.position;
        ChangeCapColor(defaultMaterial);
        isResetting = false;
    }

    private void ChangeCapColor(Material newMaterial)
    {
        if (capRenderer != null)
        {
            capRenderer.material = newMaterial;
        }
    }
}
