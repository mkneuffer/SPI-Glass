using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinInteraction : MonoBehaviour
{
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
    private Renderer capRenderer;

    void Start()
    {
        // Find the cap (child object) by name
        Transform capTransform = transform.Find(capObjectName);
        if (capTransform != null)
        {
            capRenderer = capTransform.GetComponent<Renderer>(); // Get Renderer of the cap
            capRenderer.material = defaultMaterial; // Set default material at start
        }
        else
        {
            Debug.LogError("Pin cap child not found! Check if the name in the Inspector matches.");
        }
    }

    void Update()
    {
        if (isInteracting)
        {
            // Move the pin upward
            transform.localPosition += Vector3.up * moveSpeed * Time.deltaTime;

            // Check if the pin has crossed the success hitbox
            if (successHitbox.bounds.Contains(transform.position) && !isLocked)
            {
                LockPin();
            }
        }
        else if (!isLocked)
        {
            // Gradually return the pin to the default position
            if (transform.localPosition != resetPosition.localPosition)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, resetPosition.localPosition, resetSpeed * (Time.deltaTime / 2));
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
            if (!isLocked)
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
            ChangeCapColor(defaultMaterial); // Reset cap color
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

        // Reset all pins
        PinInteraction[] allPins = FindObjectsOfType<PinInteraction>();
        foreach (PinInteraction pin in allPins)
        {
            pin.ResetPin();
        }

        // Reset counter
        pinsLocked = 0;
    }

    private void ResetPin()
    {
        isLocked = false;
        isInteracting = false;
        transform.position = resetPosition.position;
        ChangeCapColor(defaultMaterial); // Reset cap color
    }

    private void ChangeCapColor(Material newMaterial)
    {
        if (capRenderer != null)
        {
            capRenderer.material = newMaterial;
        }
    }
}
