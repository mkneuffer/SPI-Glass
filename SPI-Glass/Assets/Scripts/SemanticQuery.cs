using System.Collections;
using System.Collections.Generic;
using Niantic.Lightship.AR.Semantics;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class SemanticQuery : MonoBehaviour
{
    public ARCameraManager cameraManager;
    public ARSemanticSegmentationManager segmentationManager;

    private string channel = "ground";

    [SerializeField] Transform spawnObjectParent;
    public List<ChannelToObject> ChannelToObjects;
    [SerializeField] private InventoryManager inventoryManager;
    public int woodCount;
    [SerializeField] public int woodNeededToCraftGrail = 5;
    [SerializeField] private Animator transition;

    private void OnEnable()
    {
        woodCount = 0;
        //cameraManager.frameReceived += CameraManagerOnFrameReceived;
    }

    private void CameraManagerOnFrameReceived(ARCameraFrameEventArgs args)
    {
        // if (!segmentationManager.subsystem.running)
        // {
        //     return;
        // }


        // Matrix4x4 mat = Matrix4x4.identity;
        // var texture = segmentationManager.GetSemanticChannelTexture(channel, out mat);

        // if (texture)
        // {
        //     Matrix4x4 cameraMatrix = args.displayMatrix ?? Matrix4x4.identity;  
        // }
    }

    private float timer = 0.0f;
    // Update is called once per frame
    void Update()
    {
        if (!segmentationManager.subsystem.running)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            var pos = Input.mousePosition;
            if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
            {
                Touch touch = Input.GetTouch(0);
                pos = touch.position;
            }

            if (pos.x > 0 && pos.x < Screen.width && pos.y > 0 && pos.y < Screen.height)
            {
                timer += Time.deltaTime;
                if (timer > 0.05f)
                {
                    var list = segmentationManager.GetChannelNamesAt((int)pos.x, (int)pos.y);

                    if (list.Count > 0)
                    {
                        channel = list[0];

                        foreach (var channelToObject in ChannelToObjects)
                        {
                            if ("foliage" == channel && woodCount < woodNeededToCraftGrail)
                            {
                                woodCount++;
                                //inventoryManager.addItem(channelToObject.item);
                                //Debug.Log($"the channel {channel} has been detected and will spawn an object");
                                GameObject newObject = Instantiate(channelToObject.item.GetGameObject(), pos, Quaternion.identity, spawnObjectParent);
                                Destroy(newObject, 3f);
                            }
                        }
                    }
                    timer = 0.0f;
                }
            }
        }
        //Have gotten the required amount of wood
        if (woodCount >= woodNeededToCraftGrail)
        {
            StartCoroutine(LoadScene3());
        }
    }
    IEnumerator LoadScene3()
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(3f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(4);
    }

}


[System.Serializable]
public struct ChannelToObject
{
    public string channel;
    public ItemData item;
}