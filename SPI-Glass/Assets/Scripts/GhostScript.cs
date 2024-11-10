using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using Vector3 = UnityEngine.Vector3;

public class GhostMovement : MonoBehaviour
{

    float WRadius = .05f; //How close the ghost has to be to a point to count as being at that point
    int counter = 0;
    float BezierCurveT = 0; //Way through each bezier curve
    //private Vector3[] diamond = { Vector3.up, Vector3.right, Vector3.down, Vector3.left };
    public float speed;
    private Vector3 startingPosition;
    private Vector3 previousWaypoint;
    [SerializeField] private WaypointStorage waypointStorage1;
    [SerializeField] private WaypointStorage waypointStorage2;
    private WaypointStorage currentWaypoint;

    //[SerializeField] private TextMeshProUGUI ghostHealthTextUI;

    private int health = 10;


    // Start is called before the first frame update
    void Start()
    {
        //transform.Rotate(0.0f, 180.0f, 0.0f, Space.Self);
        startingPosition = transform.position;
        previousWaypoint = startingPosition;
        currentWaypoint = waypointStorage1;
        BezierCurveT = 0;
        //Invoke("SwapPath", 7);
    }
    // Update is called once per frame
    void Update()
    {
        MoveToPoints(currentWaypoint.GetWaypoints());
        HandleHealth();
    }

    //Moves the ghost along the given waypoints
    //The waypoints are relative to the starting position of the ghost
    void MoveToPoints(Vector3[] waypoints)
    {
        if (Vector3.Distance(startingPosition + waypoints[counter], transform.position) < WRadius)
        {
            previousWaypoint = transform.position;
            BezierCurveT = 0;
            counter++;
            if (counter >= waypoints.Length)
            {
                counter = 0;
            }
        }
        print("Before speed" + BezierCurveT);
        print("speed" + speed);
        BezierCurveT += speed;
        print("After speed" + BezierCurveT);
        Vector3 P0 = previousWaypoint;
        Vector3 P1 = P0;
        Vector3 P3 = startingPosition + waypoints[counter];
        Vector3 P2 = new Vector3(P0.x, P3.y, P0.z);
        //float angle += speed / (radius * Mathf.Tau) * Time.deltaTime;
        transform.position = BezierCurve(BezierCurveT, P0, P1, P2, P3);
        //transform.position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
        //transform.position = Vector3.MoveTowards(transform.position, startingPosition + waypoints[counter], Time.deltaTime * speed);

    }

    Vector3 BezierCurve(float t, Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3)
    {
        Vector3 output = Mathf.Pow((1 - t), 3) * P0 + 3 * Mathf.Pow((1 - t), 2) * t * P1 + 3 * (1 - t) * t * t * P2 + Mathf.Pow(t, 3) * P3;
        return output;
    }

    //Deals with the health value of the ghost
    //Does damage if touched/tapped
    //Ghost is destroyed if health <= 0
    void HandleHealth()
    {
        //ghostHealthTextUI.SetText("Ghost Health: " + health);
        if ((Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
        {
            health--;
            Debug.Log("Touched, Ghost Health = " + health);
            if (health <= 0)
            {
                Debug.Log("killing ghost");
                Destroy(gameObject);
            }
        }

    }

    //Swaps between two different waypoints
    void SwapPath()
    {
        counter = 0;
        if (currentWaypoint == waypointStorage1)
        {
            currentWaypoint = waypointStorage2;
        }
        else if (currentWaypoint == waypointStorage2)
        {
            currentWaypoint = waypointStorage1;
        }
    }
}
