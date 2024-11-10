using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointStorage : MonoBehaviour
{
    [SerializeField] private Waypoints[] waypoints;

    public Waypoints[] GetWaypoints()
    {
        return waypoints;
    }
}

[SerializeField]
[System.Serializable]
public struct Waypoints
{
    public Vector3 point;
    public bool curvesUp;
}