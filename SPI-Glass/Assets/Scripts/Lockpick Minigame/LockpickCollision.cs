using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinInteraction : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] Transform resetPosition;
    [SerializeField] Collider successHitbox;
    private int totalPins = 2;
    private int pinsLocked = 0;
    private bool isInteracting = false;
    private bool isLocked = false;

    void Update()
    {
        if (isInteracting && !isLocked)
        {
            // Moves pin upward
            transform.localPosition += Vector3.up * moveSpeed * Time.deltaTime;

            // Check if pin has crossed success hitbox
            if (successHitbox.bounds.Contains(transform.position))
            {
                LockPin();
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
                //Debug.Log("Pin exited hitbox. Resetting all pins.");
                ResetAllPins();
            }
        }
    }

    private void LockPin()
    {
        isLocked = true;
        pinsLocked++;
        //Debug.Log("Pin locked!");

        // Win condition when pins all locked
        if (pinsLocked == totalPins)
        {
           // Debug.Log("All pins locked!");
        }
    }

    private void ResetAllPins()
    {
        //Debug.Log("Pin went too high! Resetting all pins.");

        // Resets all pins
        PinInteraction[] allPins = FindObjectsOfType<PinInteraction>();
        foreach (PinInteraction pin in allPins)
        {
            pin.ResetPin();
        }

        // Resets counter
        pinsLocked = 0;
    }

    private void ResetPin()
    {
        // Reset pin position
        isLocked = false;
        isInteracting = false;
        transform.position = resetPosition.position;
    }
}