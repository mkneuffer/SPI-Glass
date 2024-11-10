using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    [SerializeField] private Vector3 controlPoint0;
    [SerializeField] private Vector3 controlPoint1;
    [SerializeField] private Vector3 controlPoint2;
    [SerializeField] private Vector3 controlPoint3;
    private Vector3[] controlPoints;

    void Start()
    {
        controlPoints = new Vector3[] { controlPoint0, controlPoint1, controlPoint2, controlPoint3 };
    }

    public Vector3[] GetControlPoints()
    {
        return controlPoints;
    }
}
