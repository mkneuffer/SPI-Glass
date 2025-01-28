using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 100f; // Pick speed
    [SerializeField] float pushBackForce = 0.5f; // Push force
    private Vector2 touchStartPos;

    void Update()
    {
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
            Vector3 movement = new Vector3(mouseDelta.x, mouseDelta.y, 0);
            transform.position += movement.normalized * moveSpeed * Time.deltaTime;
            touchStartPos = Input.mousePosition;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Barrier"))
        {
            // Pushes pick away from collided barrier
            Vector3 pushDirection = transform.position - other.ClosestPoint(transform.position);
            pushDirection.Normalize();

            // Applys pushback
            transform.position += pushDirection * pushBackForce;
        }
    }
}
