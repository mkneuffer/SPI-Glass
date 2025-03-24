using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEngine.UI;

public class SettingsSaving : MonoBehaviour
{
    private string path = "";
    private string persistentPath = "";
    public SettingsData settingsData;
    [SerializeField] private string inputField;
    [SerializeField] private GameObject inputBox;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private TextAsset nextText;
    // Start is called before the first frame update
    void Start()
    {
        setSettings();
        SetPaths();
        setNameFieldActive(false);
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
            SaveData();
        }
    }

    public void SaveData()
    {
        string savePath = persistentPath;

        Debug.Log("Saving Data at " + savePath);
        string settings = JsonUtility.ToJson(settingsData);
        Debug.Log(settings);


        using StreamWriter writer = new StreamWriter(savePath);
        writer.Write(settings);
    }

    public void LoadData()
    {
        using StreamReader reader = new StreamReader(persistentPath);
        string json = reader.ReadToEnd();

        SettingsData settings = JsonUtility.FromJson<SettingsData>(json);
        Debug.Log(settings.ToString());
        settingsData = settings;

    }
}
