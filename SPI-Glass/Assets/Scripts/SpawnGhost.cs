using System;
using System.Collections;
using System.Collections.Generic;
using Niantic.Experimental.Lightship.AR.WorldPositioning;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
public class MoveGhost : MonoBehaviour
{
    [SerializeField] private GameObject ghost;
    [SerializeField] private ARWorldPositioningObjectHelper objectHelper;
    [SerializeField] private ARWorldPositioningManager positioningManager;
    [SerializeField] private ARCameraManager cameraManager;


    // Start is called before the first frame update
    void Start()
    {
        Invoke("SpawnGhost", 1);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Current Pos = " + cameraManager.GetComponent<Transform>().position);
        }


    }

    //Spawns the ghost at the cameras location
    void SpawnGhost()
    {
        Instantiate(ghost, cameraManager.GetComponent<Transform>().position + Camera.main.transform.forward * 3, Quaternion.identity);
        Debug.Log("spawned ghost");
    }
}
