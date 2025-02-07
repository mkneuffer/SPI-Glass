using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class InventorySaving : MonoBehaviour
{

    private string path = "";
    private string persistentPath = "";
    public InventoryData inventoryData;
    [SerializeField] private List<ItemData> itemList;
    // Start is called before the first frame update
    void Start()
    {
        setInventory();
        SetPaths();
    }

    private void SetPaths()
    {
        path = Application.dataPath + Path.AltDirectorySeparatorChar + "InventoryData.json";
        persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "InventoryData.json";
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

    public void SaveData()
    {
        string savePath = path;

        Debug.Log("Saving Data at " + savePath);
        string inventory = JsonUtility.ToJson(inventoryData);
        Debug.Log(inventory);


        using StreamWriter writer = new StreamWriter(savePath);
        writer.Write(inventory);
    }

    public void LoadData()
    {
        using StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();

        InventoryData inventory = JsonUtility.FromJson<InventoryData>(json);
    }
}
