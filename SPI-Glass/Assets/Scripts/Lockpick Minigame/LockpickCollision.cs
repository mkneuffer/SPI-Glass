using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinInteraction : MonoBehaviour
{
    [SerializeField] Transform lockpick;
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float resetSpeed = 5f;
    [SerializeField] Transform resetPosition;
    [SerializeField] Collider successHitbox;
    [SerializeField] Material defaultMaterial; // Default cap material
    [SerializeField] Material successMaterial; // Cap material when in correct position
    [SerializeField] string capObjectName = "PinCap"; // Name of the child object for the cap

    private int totalPins = 2;
    private static int pinsLocked = 0;
    private bool isInteracting = false;
    private bool isLocked = false;
    private bool isResetting = false;
    private Renderer capRenderer;
    private Rigidbody rb;
    private PickMovement pickMovement;

    void Start()
    {
        // Find the cap (child object)
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

        // Get reference to the PickMovement script
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
        else if (!isLocked)
        {
            // Returns to default position if not locked & not interacted with
            rb.velocity = Vector3.zero; // Stop current movement
            transform.position = Vector3.MoveTowards(transform.position, resetPosition.position, resetSpeed * Time.deltaTime);
            if(transform.position.y == resetPosition.position.y)
            {
                isLocked = true;
            }
            else
            {
                isLocked = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lockpick"))
        {
            if (!isInteracting)
            {
                isInteracting = true;
            }
        }
        else if (other == successHitbox)
        {
            if (!isLocked && !isResetting)
            {
                ChangeCapColor(successMaterial); // Change cap color
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

        if (pinsLocked == totalPins)
        {
            Debug.Log("All pins locked!");
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

        StartCoroutine(resetDelay());
    }

    private void ResetPin()
    {
        isLocked = false;
        isInteracting = false;
        isResetting = true;
        rb.velocity = Vector3.zero;
        
        transform.position = Vector3.MoveTowards(transform.position, resetPosition.position, resetSpeed * Time.deltaTime);
        //transform.position = resetPosition.position;
        ChangeCapColor(defaultMaterial); // Reset cap color back to red
        isResetting = false;
    }

    private void ChangeCapColor(Material newMaterial)
    {
        if (capRenderer != null)
        {
            capRenderer.material = newMaterial; // Change cap color to red/green
        }
    }

    IEnumerator resetDelay()
    {
        yield return new WaitForSeconds(.5f);

        // Reset all pins
        PinInteraction[] allPins = FindObjectsOfType<PinInteraction>();
        foreach (PinInteraction pin in allPins)
        {
            pin.ResetPin();
        }

        // Reset counter
        pinsLocked = 0;
    }
}
