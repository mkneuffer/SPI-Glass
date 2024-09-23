using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{

    float WRadius = .05f; //How close the ghost has to be to a point to count as being at that point
    int counter = 0;
    Vector3[] diamond = { Vector3.up, Vector3.right, Vector3.down, Vector3.left };
    public float speed;
    Vector3 startingPosition;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        MoveToPoints(diamond);

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
        Debug.Log("Time.deltaTime: " + Time.deltaTime + " speed * deltaTime: " + (100f * Time.deltaTime) + " speed: " + speed);
        //Debug.Log("Current Pos: " + transform.position + " Going to: " + (startingPosition + waypoints[counter]));
        transform.position = Vector3.MoveTowards(transform.position, startingPosition + waypoints[counter], Time.deltaTime * speed);
    }

}
