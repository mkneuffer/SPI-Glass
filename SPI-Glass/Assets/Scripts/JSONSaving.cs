using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class JSONSaving : MonoBehaviour
{

    private string path = "";
    private string persistentPath = "";
    [SerializeField] private InventoryData inventoryData;
    [SerializeField] private SettingsData settingsData;
    // Start is called before the first frame update
    void Start()
    {
        setInventory();
        setSettings();
        SetPaths();
    }

    private void SetPaths()
    {
        path = Application.dataPath + Path.AltDirectorySeparatorChar + "SaveData.json";
        persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SaveData.json";
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            SaveData();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadData();
        }
    }
    
    private void setInventory()
    {
        if (inventoryData == null)
        {
            inventoryData = new InventoryData(null);
        }
        
    }

    private void setSettings()
    {
        settingsData = new SettingsData(50f);
    }

    public void SaveData()
    {
        string savePath = path;

        Debug.Log("Saving Data at " + savePath);
        string inventory = JsonUtility.ToJson(inventoryData);
        Debug.Log(inventory);

        string settings = JsonUtility.ToJson(settingsData);
        Debug.Log(settings);

        using StreamWriter writer = new StreamWriter(savePath);
        writer.Write(inventory);
        writer.Write(settings);
    }

    public void LoadData()
    {
        using StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();

        InventoryData inventory = JsonUtility.FromJson<InventoryData>(json);
    }
}
