using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsData : MonoBehaviour
{
    public float volume;
    public string name;
    public bool scene;


    public SettingsData(float volume, string name)
    {
        this.volume = volume;
        this.name = name;
        this.scene = false;
    }

    private void Awake()
    {
        var settingsDataList = FindObjectsOfType<SettingsData>();
        if (settingsDataList.Length > 1)
        {
            for (int i = 0; i < settingsDataList.Length; i++)
            {
                Debug.Log("multiple settings data found at " + settingsDataList[i].transform.name);
            }
            
            Destroy(this.gameObject);  // Destroy the duplicate
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public override string ToString()
    {
        return ("volume set to: " + volume);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float getVolume()
    {
        return volume;
    }
    public string getName()
    {
        return name;
    }

    public void sceneCheck()
    {
        Debug.Log("changing scene");
        scene = true;
    }

    public bool getScene()
    {
        return scene;
    }

    public void setName(string newName)
    {
        name = newName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
