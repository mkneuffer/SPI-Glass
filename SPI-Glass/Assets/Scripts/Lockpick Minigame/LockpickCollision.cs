using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockpickCollision : MonoBehaviour
{
    [SerializeField] GameObject Pin;        // The pin that moves
    [SerializeField] float moveSpeed = 1f; // Speed of the pin's upward movement
    [SerializeField] float maxHeight = 2f; // Maximum height the pin can move to

    private bool canMovePin = false;
    private bool isPinLocked = false;
    private int totalPins = 2;
    private int pinsLocked;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the lockpick collides with the pin
        if (collision.gameObject.CompareTag("Lockpick") && !isPinLocked)
        {
            //Debug.Log("Lockpick collided with the pin");

            // Check if the pin can still move upwards
            if (Pin.transform.position.y < maxHeight)
            {
                canMovePin = true;
            }
        }

        // Check if the pin collides with the upper barrier
        if (collision.gameObject.CompareTag("UpperBarrier") && isPinLocked == false)
        {
            Debug.Log("Pin reached the upper barrier and is locked in place");
            isPinLocked = true;
            canMovePin = false; // Prevent further movement
            pinsLocked++;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // While the lockpick is in contact, move the pin upwards
        if (canMovePin && collision.gameObject.CompareTag("Lockpick") && !isPinLocked)
        {
            Vector3 currentPosition = Pin.transform.position;

            // Move the pin upwards
            if (currentPosition.y < maxHeight)
            {
                Pin.transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            }
            else
            {
                canMovePin = false; // Stop movement when max height is reached
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Stop movement when the lockpick exits collision
        if (collision.gameObject.CompareTag("Lockpick"))
        {
            //Debug.Log("Lockpick exited collision with the pin");
            canMovePin = false;
        }
    }

    private void checkPins()
    {
        if(pinsLocked == totalPins)
        {
            Debug.Log("Finished!");
        }
    }
}
