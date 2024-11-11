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

    public TMP_Text text;
    public RawImage image;
    public Material material;

    private string channel = "ground";

    [SerializeField] Transform spawnObjectParent;
    public List<ChannelToObject> ChannelToObjects;

    private void OnEnable()
    {
        image.enabled = false;
        cameraManager.frameReceived += CameraManagerOnFrameReceived;
    }

    private void CameraManagerOnFrameReceived(ARCameraFrameEventArgs args)
    {
        if (!segmentationManager.subsystem.running)
        {
            return;
        }


        Matrix4x4 mat = Matrix4x4.identity;
        var texture = segmentationManager.GetSemanticChannelTexture(channel, out mat);

        if (texture)
        {
            Matrix4x4 cameraMatrix = args.displayMatrix ?? Matrix4x4.identity;
            image.material = material;
            image.material.SetTexture("_SemanticTex", texture);
            image.material.SetMatrix("_SemanticMat", mat);


        }
    }

    private float timer = 0.0f;
    // Update is called once per frame
    void Update()
    {
        if (!segmentationManager.subsystem.running)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) || Input.touches.Length > 0)
        {
            var pos = Input.mousePosition;

            if (pos.x > 0 && pos.x < Screen.width && pos.y > 0 && pos.y < Screen.height)
            {
                timer += Time.deltaTime;
                image.enabled = true;
                if (timer > 0.05f)
                {
                    var list = segmentationManager.GetChannelNamesAt((int)pos.x, (int)pos.y);

                    if (list.Count > 0)
                    {
                        channel = list[0];
                        text.text = channel;

                        foreach (var channelToObject in ChannelToObjects)
                        {
                            if (channelToObject.channel == channel)
                            {
                                Debug.Log($"the channel {channel} has been detected and will spawn an object");
                                GameObject newObject = Instantiate(channelToObject.GameObject, pos, Quaternion.identity, spawnObjectParent);
                                Destroy(newObject, 3f);
                            }
                        }
                    }
                    else
                    {
                        text.text = "?";
                    }

                    timer = 0.0f;
                }
            }
        }
    }
}

[System.Serializable]
public struct ChannelToObject
{
    public string channel;
    public GameObject GameObject;
}