using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointStorage : MonoBehaviour
{
    [SerializeField] private Vector3[] waypoints = { };

    public Vector3[] GetWaypoints()
    {
        return waypoints;
    }
}
