using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEngine.UI;

public class SettingsSaving : MonoBehaviour
{
    private string persistentPath = "";
    public SettingsData settingsData;
    [SerializeField] private string inputField;
    [SerializeField] private GameObject inputBox;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private TextAsset nextText;
    // Start is called before the first frame update

    void Start()
    {
        SetPaths();
        setNameFieldActive(false);
    }

    private void Awake()
    {
        LoadData();
        setSettings();
    }

    private void SetPaths()
    {
        persistentPath = Application.dataPath + Path.AltDirectorySeparatorChar + "SaveData.json";
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void setSettings()
    {
        
        if (settingsData == null)
        {
            settingsData = new SettingsData(50f, "0");
            Debug.Log("default settings enabled with volume: " + settingsData.getVolume());
        }
        
    }
    public void setName(string name)
    {
        inputField = name;
        settingsData.setName(inputField);
        Debug.Log("name changed to: " + inputField);
    }

    public string getName()
    {
        return settingsData.getName();
    }

    public void setNameFieldActive(bool state)
    {
        if (inputBox != null)
        {
            inputBox.SetActive(state);
        }
    }

    public void finishName()
    {
        if (inputBox != null)
        {
            
            Vector3 newBoxPos = new Vector3(10000, 10000, 0);
            inputBox.transform.position = newBoxPos;
            dialogueManager.EnterDialogueMode(nextText, true);
            Debug.Log("saving data");
            SaveData();
            Debug.Log("saved");
        }
    }

    public void SaveData()
    {
        try
        {
            Debug.Log("Saving Data at " + persistentPath);
            string settingsJson = JsonUtility.ToJson(settingsData);
            Debug.Log(settingsJson);

            // Use StreamWriter to save the settings in the persistent data path
            File.WriteAllText(persistentPath, settingsJson);  // File.WriteAllText is a simpler method for saving text files

            Debug.Log("Data saved successfully.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error saving data: " + ex.Message);
        }
    }

    public void LoadData()
    {
        try
        {
            if (File.Exists(persistentPath))  // Check if the file exists before attempting to load
            {
                Debug.Log("loading data");
                string json = File.ReadAllText(persistentPath);
                SettingsData loadedSettings = JsonUtility.FromJson<SettingsData>(json);
                Debug.Log("Loaded settings: " + loadedSettings.ToString());
                settingsData = loadedSettings;
            }
            else
            {
                Debug.LogWarning("Settings file not found at " + persistentPath);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error loading data: " + ex.Message);
        }
    }
}
