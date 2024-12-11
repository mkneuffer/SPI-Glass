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
    public bool startFight = false;


    // Start is called before the first frame update
    void Start()
    {
        Invoke("PutGhostInSpawn", 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (startFight == true)
        {
            //Invoke("PutGhostInSpawn", 1);

            startFight = false;
        }

    }

    //Spawns the ghost at the cameras location
    /*void SpawnGhost()
    {
        Quaternion rotation = Quaternion.identity;
        rotation.Set(rotation.x, Camera.main.transform.rotation.y + 180.0f, rotation.z, rotation.w);
        Instantiate(ghost, cameraManager.GetComponent<Transform>().position + Camera.main.transform.forward * 3, rotation);
    }*/

    void PutGhostInSpawn()
    {
        // Debug.Log(ghost.transform.position);
        Quaternion rotation = Quaternion.identity;
        rotation.Set(rotation.x, Camera.main.transform.rotation.y + 180.0f, rotation.z, rotation.w);
        ghost.transform.position = cameraManager.GetComponent<Transform>().position + Camera.main.transform.forward * 3;
        ghost.transform.rotation = rotation;
        // Debug.Log(ghost.transform.position);
        ghost.SetActive(true);

    }

    public void startGhostFight()
    {
        startFight = true;
    }
}
