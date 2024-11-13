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
    private GhostScript ghostScript;
    [SerializeField] private ARWorldPositioningObjectHelper objectHelper;
    [SerializeField] private ARWorldPositioningManager positioningManager;
    [SerializeField] private ARCameraManager cameraManager;


    // Start is called before the first frame update
    void Start()
    {
        Invoke("SpawnGhost", 1);
        ghostScript = ghost.GetComponent<GhostScript>();
    }

    // Update is called once per frame
    void Update()
    {
        // if ((Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
        // {
        //     ghostScript.setFollowPath(false);
        //     ghostScript.MoveTo(ghostScript.transform.position + new Vector3(0, 3, 0), 1);
        // }

    }

    //Spawns the ghost at the cameras location
    void SpawnGhost()
    {
        Quaternion rotation = Quaternion.identity;
        rotation.Set(rotation.x, Camera.main.transform.rotation.y + 180.0f, rotation.z, rotation.w);
        ghost = Instantiate(ghost, cameraManager.GetComponent<Transform>().position + Camera.main.transform.forward * 3, rotation);
        ghostScript = ghost.GetComponent<GhostScript>();
    }
}
