using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefFightManager : MonoBehaviour
{
    private string currentlySelectedItem = "";
    [SerializeField] private ARLaserAndMirrorManager laserManager;
    [SerializeField] private Launcher launcher;
    // Start is called before the first frame update
    void Start()
    {
        launcher.enabled = false;
        laserManager.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectItem(string itemName)
    {
        if (itemName.Equals(currentlySelectedItem))
        {
            Debug.Log("Item (" + itemName + ") already selected");
            return;
        }
        Debug.Log("Selecting Item: " + itemName);

        if (itemName.Equals("rope"))
        {
            launcher.enabled = true;
            currentlySelectedItem = itemName;
            laserManager.SetActive(false);

        }
        else if (itemName.Equals("laser"))
        {
            launcher.enabled = false;
            currentlySelectedItem = itemName;
            laserManager.SetPrefabType2();
            laserManager.SetActive(true);
        }
        else if (itemName.Equals("mirror"))
        {
            launcher.enabled = false;
            currentlySelectedItem = itemName;
            laserManager.SetPrefabType1();
            laserManager.SetActive(true);
        }
    }
}
