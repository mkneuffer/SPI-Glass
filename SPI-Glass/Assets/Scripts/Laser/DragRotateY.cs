using UnityEngine;

public class DragRotateY : MonoBehaviour
{
    public float rotationSpeed = 5.0f; // Adjust rotation sensitivity
    private bool isDragging = false;
    private Vector2 lastTouchPosition;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Handle touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            HandleTouch(touch);
        }
        // Handle mouse input
        else if (Input.GetMouseButtonDown(0))
        {
            if (IsTouchingObject(Input.mousePosition))
            {
                isDragging = true;
                lastTouchPosition = Input.mousePosition;
            }
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            HandleDrag(Input.mousePosition, TouchPhase.Moved);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    void HandleTouch(Touch touch)
    {
        if (touch.phase == TouchPhase.Began && IsTouchingObject(touch.position))
        {
            isDragging = true;
            lastTouchPosition = touch.position;
        }
        else if (touch.phase == TouchPhase.Moved && isDragging)
        {
            HandleDrag(touch.position, TouchPhase.Moved);
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            isDragging = false;
        }
    }

    void HandleDrag(Vector2 currentPosition, TouchPhase phase)
    {
        if (phase == TouchPhase.Moved && isDragging)
        {
            Vector2 delta = currentPosition - lastTouchPosition;
            float rotationAmount = delta.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, -rotationAmount, Space.World);
            lastTouchPosition = currentPosition;
        }
    }

    bool IsTouchingObject(Vector2 touchPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(touchPosition);
        RaycastHit hit;
        return Physics.Raycast(ray, out hit) && hit.transform == transform;
    }
}
