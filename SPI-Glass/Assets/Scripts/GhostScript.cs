using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class GhostMovement : MonoBehaviour
{

    float WRadius = .05f; //How close the ghost has to be to a point to count as being at that point
    int counter = 0;
    private Vector3[] diamond = { Vector3.up, Vector3.right, Vector3.down, Vector3.left };
    public float speed;
    private Vector3 startingPosition;
    [SerializeField] private WaypointStorage waypointStorage1;
    [SerializeField] private WaypointStorage waypointStorage2;
    private WaypointStorage currentWaypoint;

    //[SerializeField] private TextMeshProUGUI ghostHealthTextUI;

    private int health = 10;


    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
        currentWaypoint = waypointStorage1;
        Invoke("SwapPath", 7);
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
            counter++;
            if (counter >= waypoints.Length)
            {
                counter = 0;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, startingPosition + waypoints[counter], Time.deltaTime * speed);
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
