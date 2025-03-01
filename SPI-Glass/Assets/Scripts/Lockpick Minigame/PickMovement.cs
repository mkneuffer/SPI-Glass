using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed; // Pick speed
    [SerializeField] Transform pickResetPosition;

    private Vector2 touchStartPos;
    private Vector3 previousPosition;
    public Vector3 velocity;
    //private Vector3 targetVelocity;

    private bool isTouching = false;
    private bool canMoveUp = true;
    private bool canMoveDown = true;
    private bool canMoveRight = true;
    private bool canMoveLeft = true;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        previousPosition = transform.position;
    }

    void Update()
    {
        HandleTouchInput();
        velocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;
    }

    void HandleTouchInput()
    {
        //Vector3 movement = Vector3.zero;
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                isTouching = true;
            }
            else if (touch.phase == TouchPhase.Moved && isTouching)
            {
                Vector2 touchDelta = touch.deltaPosition * 0.05f;
                Vector3 movement = new Vector3(touchDelta.x, touchDelta.y, 0).normalized * moveSpeed;
                //rb.velocity = movement.normalized * moveSpeed;

                if ((!canMoveUp && touchDelta.y > 0) ||
                    (!canMoveDown && touchDelta.y < 0) ||
                    (!canMoveRight && touchDelta.x > 0) ||
                    (!canMoveLeft && touchDelta.x < 0))
                {
                    movement = Vector3.zero; // Stop movement if colliding
                }

                rb.velocity = movement; // Use Rigidbody velocity for movement
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isTouching = false;
                rb.velocity = Vector3.zero;
            }
        }
    }

/*
    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 mouseDelta = (Vector2)Input.mousePosition - touchStartPos;
            if(canMoveUp && canMoveDown && canMoveRight && canMoveLeft) // Collisions for mouse detection
            {
                Vector3 movement = new Vector3(mouseDelta.x, mouseDelta.y, 0);
                transform.position += movement.normalized * moveSpeed * Time.deltaTime;
                touchStartPos = Input.mousePosition;
            }
            else if(!canMoveUp && mouseDelta.y < 0)
            {
                Vector3 movement = new Vector3(mouseDelta.x, mouseDelta.y, 0);
                transform.position += movement.normalized * moveSpeed * Time.deltaTime;
                touchStartPos = Input.mousePosition;
            }
            else if(!canMoveDown && mouseDelta.y > 0)
            {
                Vector3 movement = new Vector3(mouseDelta.x, mouseDelta.y, 0);
                transform.position += movement.normalized * moveSpeed * Time.deltaTime;
                touchStartPos = Input.mousePosition;
            }
            else if(!canMoveRight && mouseDelta.x < 0)
            {
                Vector3 movement = new Vector3(mouseDelta.x, mouseDelta.y, 0);
                transform.position += movement.normalized * moveSpeed * Time.deltaTime;
                touchStartPos = Input.mousePosition;
            }
            else if(!canMoveLeft && mouseDelta.x > 0)
            {
                Vector3 movement = new Vector3(mouseDelta.x, mouseDelta.y, 0);
                transform.position += movement.normalized * moveSpeed * Time.deltaTime;
                touchStartPos = Input.mousePosition;
            }
        }
    }
 */
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("UpperBarrier"))
        {
            //Debug.Log("Collided with upper barrier");
            canMoveUp = false;
        }
        if(other.CompareTag("LowerBarrier"))
        {
            //Debug.Log("Collided with lower barrier");
            canMoveDown = false;
        }
        if(other.CompareTag("SideBarrier"))
        {
            //Debug.Log("Collided with side barrier");
            canMoveRight = false;
        }
        if(other.CompareTag("Boundary"))
        {
            //Debug.Log("Collided with boundary");
            canMoveLeft = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("UpperBarrier"))
        {
            //Debug.Log("Exited upper barrier");
            canMoveUp = true;
        }
        if(other.CompareTag("LowerBarrier"))
        {
            //Debug.Log("Exited lower barrier");
            canMoveDown = true;
        }
        if(other.CompareTag("SideBarrier"))
        {
            //Debug.Log("Exited side barrier");
            canMoveRight = true;
        }
        if(other.CompareTag("Boundary"))
        {
            //Debug.Log("Exited boundary");
            canMoveLeft = true;
        }
    }

public void ResetPickPosition()
{
        Debug.Log("Resetting pick position.");
        transform.position = pickResetPosition.position;
        rb.velocity = Vector3.zero; // Temporarily pause movement
}
}
