using System.Collections;
using System.Collections.Generic;
using Niantic.Lightship.AR.Semantics;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[System.Serializable]
public struct ChannelToObject
{
    public string channel;
    public GameObject item;
}

public class SemanticQuery : MonoBehaviour
{
    public ARCameraManager cameraManager;
    public ARSemanticSegmentationManager segmentationManager;

    private string channel = "ground";

    [SerializeField] Transform spawnObjectParent;
    public List<ChannelToObject> ChannelToObjects;
    [SerializeField] private InventoryManager inventoryManager;
    public int woodCount;
    public int woodInProgressBar;
    [SerializeField] public int woodNeededToCraftGrail = 5;
    [SerializeField] private float woodSpeed = 3700;
    [SerializeField] private Animator transition;
    [SerializeField] private bool testing = false;
    private int touchTime = 0;

    [SerializeField] private XR_Placement xrPlacement; // Reference to the XR_Placement script

    private float actionInterval = 0.05f; // Limit processing to every 50ms
    private float lastActionTime = 0.0f;

    private List<GameObject> woodObjects = new List<GameObject>();
    private Queue<GameObject> woodPool = new Queue<GameObject>();
    private bool allowTapping = true;

    private void OnEnable()
    {
        woodCount = 0;
        woodInProgressBar = 0;
        InitializeWoodPool(10); // Preload wood objects
    }

    private void InitializeWoodPool(int count)
    {
        foreach (var channelToObject in ChannelToObjects)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject obj = Instantiate(channelToObject.item, spawnObjectParent);
                obj.SetActive(false);
                woodPool.Enqueue(obj);
            }
        }
    }

    private GameObject GetPooledObject()
    {
        if (woodPool.Count > 0)
        {
            GameObject obj = woodPool.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        Debug.LogWarning("Wood pool is empty, consider increasing pool size.");
        return null;
    }

    private void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        woodPool.Enqueue(obj);
    }

    void Update()
    {
        if (!segmentationManager.subsystem.running || Time.time - lastActionTime < actionInterval)
        {
            return;
        }

        lastActionTime = Time.time;

        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) && allowTapping)
        {
            allowTapping = false;
            Vector2 pos = Input.mousePosition;
            StartCoroutine(AllowTappingTime());
            if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began && touchTime == 0)
            {
                Touch touch = Input.GetTouch(0);
                pos = touch.position;
                touchTime++;
            }
            touchTime = 0;

            StartCoroutine(ProcessSegmentation(pos));
        }

        Vector3 goingTo = new Vector3(Screen.width - 195, Screen.height - 1 - 150, -1);
        foreach (GameObject wood in woodObjects)
        {
            if (wood != null && wood.activeSelf)
            {
                wood.transform.position = Vector3.MoveTowards(wood.transform.position, goingTo, Time.deltaTime * woodSpeed);
                if (Vector3.Distance(goingTo, wood.transform.position) < 100)
                {
                    wood.transform.position = new Vector3();
                    ReturnToPool(wood);
                    woodInProgressBar++;
                }
            }
        }

        // Trigger the prefab spawn when enough wood is collected
        if (woodInProgressBar >= woodNeededToCraftGrail)
        {
            if (xrPlacement != null)
            {
                xrPlacement.SpawnGhost();
            }
            else
            {
                Debug.LogWarning("XR_Placement reference is not set!");
            }
        }
    }

    //Sets a 0.5 second delay between when it allows tapping
    private IEnumerator AllowTappingTime()
    {
        yield return new WaitForSeconds(.1f);
        allowTapping = true;
    }

    private IEnumerator ProcessSegmentation(Vector2 pos)
    {
        var list = segmentationManager.GetChannelNamesAt((int)pos.x, (int)pos.y);
        yield return null; // Allow frame to process before continuing

        if (list.Count > 0)
        {
            channel = list[0];
            foreach (var channelToObject in ChannelToObjects)
            {
                if (("foliage" == channel || testing) && woodCount < woodNeededToCraftGrail)
                {
                    woodCount++;
                    GameObject newObject = GetPooledObject();
                    if (newObject != null)
                    {
                        newObject.transform.position = pos;
                        newObject.transform.rotation = Quaternion.identity;
                        woodObjects.Add(newObject);
                    }
                }
            }
        }
    }

    IEnumerator LoadScene4()
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(3f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(4);
    }
}
