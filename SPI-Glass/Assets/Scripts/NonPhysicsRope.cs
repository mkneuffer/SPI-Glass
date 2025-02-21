using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class NonPhysicsRope : MonoBehaviour
{
    [SerializeField] float force = 1000f;
    [SerializeField] float angle = 45;
    [SerializeField] ARCameraManager arCamera;
    bool anchoredToCamera = true;

    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(90, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (anchoredToCamera)
        {
            transform.position = arCamera.transform.position + arCamera.transform.forward;
        }
    }

    public void Shoot()
    {
        anchoredToCamera = false;
        Debug.Log("force applied");
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        Vector3 forceVector = arCamera.transform.forward * force;
        rigidbody.AddForce(forceVector);
        rigidbody.useGravity = true;
    }

    public void Reset()
    {
        anchoredToCamera = true;
    }
}
