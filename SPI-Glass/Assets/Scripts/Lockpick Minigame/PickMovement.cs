using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3f; // Pick speed
    [SerializeField] Transform pickResetPosition;
    private Vector2 touchStartPos;
    private Vector3 lastPosition;
    public Vector3 velocity { get; private set; }
    private bool canMoveUp = true;
    private bool canMoveDown = true;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        lastPosition = transform.position;
    }

    void Update()
    {
        velocity = (transform.position - lastPosition) / Time.deltaTime; // Calculate velocity
        lastPosition = transform.position; // Store position for next frame

        #if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
        #else
        HandleTouchInput();
        #endif
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 touchDelta = touch.position - touchStartPos;
                Vector3 movement = new Vector3(touchDelta.x, touchDelta.y, 0);
                transform.position += movement.normalized * moveSpeed * Time.deltaTime;
                touchStartPos = touch.position;
            }
        }
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 mouseDelta = (Vector2)Input.mousePosition - touchStartPos;
            if(canMoveUp && canMoveDown)
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
        }
    }

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
    }

public void ResetPickPosition()
{
    Debug.Log("Resetting pick position.");
    transform.position = pickResetPosition.position;
    rb.velocity = Vector3.zero; // Stop any movement
}
}
