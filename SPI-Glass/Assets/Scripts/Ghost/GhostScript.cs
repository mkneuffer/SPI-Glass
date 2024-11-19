using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GhostMovement : MonoBehaviour
{

    float WRadius = .05f; //How close the ghost has to be to a point to count as being at that point
    int counter = 0;
    float BezierCurveT = 0; //Way through each bezier curve
    //private Vector3[] diamond = { Vector3.up, Vector3.right, Vector3.down, Vector3.left };
    public float speed; //Value should be around .01
    private Vector3 startingPosition;
    private Vector3 previousWaypoint;
    [SerializeField] private WaypointStorage waypointStorage1;
    [SerializeField] private WaypointStorage waypointStorage2;
    private WaypointStorage currentWaypoint;
    private Vector3 previousPosition;

    //[SerializeField] private TextMeshProUGUI ghostHealthTextUI;

    private int health = 10;
    private int phase = 1;
    public bool isStunned = false;


    // Start is called before the first frame update
    void Start()
    {
        //transform.Rotate(0.0f, 180.0f, 0.0f, Space.Self);
        startingPosition = transform.position;
        previousWaypoint = startingPosition;
        currentWaypoint = waypointStorage1;
        previousPosition = startingPosition;
        BezierCurveT = 0;
        //Invoke("SwapPath", 7);
    }
    // Update is called once per frame
    void Update()
    {
            MoveToPoints(currentWaypoint.GetWaypoints());
            if(!isStunned) {
                MoveToPoints(currentWaypoint.GetWaypoints());
            }

            if(isStunned) {
                if ((Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
                {
                    HandleHealth(1);
                }
            }
    }

    //Moves the ghost along the given waypoints
    //The waypoints are relative to the starting position of the ghost
    void MoveToPoints(Waypoints[] waypoints)
    {

        if (Vector3.Distance(startingPosition + waypoints[counter].point, transform.position) < WRadius)
        {

            previousWaypoint = transform.position;
            BezierCurveT = speed;
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
        //Debug.Log("P0:" + P0 + ", P1:" + P1 + ", P2:" + P2 + ", P3:" + P3);
        // if (Mathf.Abs(P0.x - startingPosition.x) > Mathf.Abs(P3.x - startingPosition.x))
        // {
        //     P1.x = P0.x;

        // }
        // else
        // {
        //     //P1.x 
        //     P2.x = P3.x;
        // }

        // if (Mathf.Abs(P0.y - startingPosition.y) > Mathf.Abs(P3.y - startingPosition.y))
        // {
        //     P2.y = P0.y;
        // }
        // else
        // {
        //     P2.y = P3.y;
        // }

        // if (Mathf.Abs(P0.z - startingPosition.z) > Mathf.Abs(P3.z - startingPosition.z))
        // {
        //     P2.z = P0.z;
        // }
        // else
        // {
        //     P2.z = P3.z;
        // }
        previousPosition = transform.position;
        transform.position = BezierCurve(BezierCurveT, P0, P1, P2, P3);
        //print("T" + BezierCurveT);
        BezierCurveT += speed;

    }



    Vector3 BezierCurve(float t, Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3)
    {
        Vector3 output = Mathf.Pow((1 - t), 3) * P0 + 3 * Mathf.Pow((1 - t), 2) * t * P1 + 3 * (1 - t) * t * t * P2 + Mathf.Pow(t, 3) * P3;
        return output;
    }

    public void SetSpeed(float change)
    {
        speed = change;
    }

    float GetSpeed()
    {
        return speed;
    }

    void multSpeed(float mult)
    {
        speed *= mult;
    }

    //Deals with the health value of the ghost
    //Does damage if touched/tapped
    //Ghost is destroyed if health <= 0
    public void HandleHealth(int amount)
    {
        health -= amount;
        Debug.Log("Health: " + health);
        if(health <= 0) {
            phase++;
            Debug.Log("Phase:" + phase);
            if(phase > 3) {
                Debug.Log("End fight");
                Destroy(gameObject);
                UnityEngine.SceneManagement.SceneManager.LoadScene(7);
            } else {
                health = 10;
                Debug.Log("Phase: " + phase);
                SwapPath();
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

    public void StunGhost(float stunTime) {
        if(!isStunned) {
            isStunned = true;
            StartCoroutine(StunCoroutine(stunTime));
        }
    }

    private IEnumerator StunCoroutine(float stunTime) {
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
        Debug.Log("Ghost recovered!");
    }
}
