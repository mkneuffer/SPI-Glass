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

public class GhostScript : MonoBehaviour
{

    float WRadius = .05f; //How close the ghost has to be to a point to count as being at that point
    int counter = 0;
    float BezierCurveT = 0; //Way through each bezier curve
    //private Vector3[] diamond = { Vector3.up, Vector3.right, Vector3.down, Vector3.left };
    public float pathSpeed; //Value should be around .02
    float regularSpeed;
    private Vector3 startingPosition;
    private Vector3 previousWaypoint;
    [SerializeField] private WaypointStorage waypointStorage1;
    [SerializeField] private WaypointStorage waypointStorage2;
    private WaypointStorage currentWaypoint;
    private Vector3 movingTo;
    private bool followPath;
    private bool manualMoving;

    //[SerializeField] private TextMeshProUGUI ghostHealthTextUI;

    private int health = 10;


    // Start is called before the first frame update
    void Start()
    {
        //transform.Rotate(0.0f, 180.0f, 0.0f, Space.Self);
        startingPosition = transform.position;
        previousWaypoint = startingPosition;
        currentWaypoint = waypointStorage1;
        followPath = true;
        manualMoving = false;
        movingTo = startingPosition;
        BezierCurveT = 0;
        //Invoke("SwapPath", 7);
    }
    // Update is called once per frame
    void Update()
    {
        if (followPath)
        {
            MoveToWaypoints(currentWaypoint.GetWaypoints());
        }
        else if (manualMoving)
        {
            MoveToPoint();
        }
        HandleHealth();
    }

    //Moves the ghost along the given waypoints
    //The waypoints are relative to the starting position of the ghost
    void MoveToWaypoints(Waypoints[] waypoints)
    {
        if (Vector3.Distance(startingPosition + waypoints[counter].point, transform.position) < WRadius)
        {

            previousWaypoint = transform.position;
            BezierCurveT = pathSpeed;
            counter++;
            if (counter >= waypoints.Length)
            {
                counter = 0;
            }
        }

        Vector3 P0 = previousWaypoint;
        Vector3 P3 = startingPosition + waypoints[counter].point;

        Vector3 P1 = new Vector3();
        Vector3 P2 = new Vector3();
        Vector3 midpoint = (P0 + P3) / 2;
        if (waypoints[counter].curvesUp)
        {

            P1 = new Vector3(midpoint.x, P0.y, midpoint.z);
            P2 = new Vector3(P3.x, midpoint.y, P3.z);
        }
        else
        {
            P1 = new Vector3(P0.x, midpoint.y, P0.z);
            P2 = new Vector3(midpoint.x, P3.y, midpoint.z);
        }
        transform.position = BezierCurve(BezierCurveT, P0, P1, P2, P3);
        //print("T" + BezierCurveT);
        BezierCurveT += pathSpeed * Time.deltaTime * 60;

    }

    //Does bezier curve stuff
    Vector3 BezierCurve(float t, Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3)
    {
        Vector3 output = Mathf.Pow((1 - t), 3) * P0 + 3 * Mathf.Pow((1 - t), 2) * t * P1 + 3 * (1 - t) * t * t * P2 + Mathf.Pow(t, 3) * P3;
        return output;
    }

    public void SetSpeed(float speed)
    {
        this.pathSpeed = speed;
    }

    public float GetSpeed()
    {
        return pathSpeed;
    }

    public void multSpeed(float mult)
    {
        this.pathSpeed *= mult;
    }

    //whether the ghost does the follow set path thing for during fights
    public void setFollowPath(bool followPath)
    {
        this.followPath = followPath;
    }

    public bool getFollowPath()
    {
        return followPath;
    }

    //Moves the ghost to the point specified at movingTo
    private void MoveToPoint()
    {
        transform.position = Vector3.MoveTowards(transform.position, movingTo, Time.deltaTime * regularSpeed);
        if (Vector3.Distance(movingTo, transform.position) < WRadius)
        {
            manualMoving = false;
        }
    }

    public void setPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    //Moves the ghost to the given point at the given speed
    //Make sure you set follow path to false before using this
    public void MoveTo(Vector3 pos, float speed)
    {
        manualMoving = true;
        regularSpeed = speed;
        movingTo = pos;
    }

    //Deals with the health value of the ghost
    //Does damage if touched/tapped
    //Ghost is destroyed if health <= 0
    void HandleHealth()
    {
        //ghostHealthTextUI.SetText("Ghost Health: " + health);
        // if ((Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
        // {
        //     health--;
        //     Debug.Log("Touched, Ghost Health = " + health);
        //     if (health <= 0)
        //     {
        //         Debug.Log("killing ghost");
        //         Destroy(gameObject);
        //     }
        // }

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
