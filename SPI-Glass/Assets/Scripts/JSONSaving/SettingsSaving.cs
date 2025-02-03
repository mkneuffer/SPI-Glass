using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class SettingsSaving : MonoBehaviour
{
    private string path = "";
    private string persistentPath = "";
    public SettingsData settingsData;
    // Start is called before the first frame update
    void Start()
    {
        setSettings();
        SetPaths();
    }

    private void SetPaths()
    {
        path = Application.dataPath + Path.AltDirectorySeparatorChar + "SettingsData.json";
        persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SettingsData.json";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveData();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadData();
        }
    }

    private void setSettings()
    {
        if (settingsData == null)
        {
            settingsData = new SettingsData(50f);
            Debug.Log("default settings enabled with volume: " + settingsData.getVolume());
        }

    }

    public void SaveData()
    {
        string savePath = path;

        Debug.Log("Saving Data at " + savePath);
        string settings = JsonUtility.ToJson(settingsData);
        Debug.Log(settings);


        using StreamWriter writer = new StreamWriter(savePath);
        writer.Write(settings);
    }

    public void LoadData()
    {
        using StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();

        SettingsData settings = JsonUtility.FromJson<SettingsData>(json);
        Debug.Log(settings.ToString());

    }
}
