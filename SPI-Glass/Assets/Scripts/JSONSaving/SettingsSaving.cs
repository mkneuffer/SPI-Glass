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

    void Awake()
    {
        settingsData = FindObjectOfType<SettingsData>();

        if (FindObjectsOfType<SettingsSaving>().Length > 1 && settingsData.getScene() == true)
        {
            Debug.Log("multiple settings saving found");
            Destroy(gameObject);  // Destroy the duplicate
        }
        else
        {
            DontDestroyOnLoad(gameObject);  // Make this object persist across scenes
        }
    }

    void Start()
    {
        setNameFieldActive(false);
    }

    // Update is called once per frame
    void Update()
    {

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
            settingsData.setName(inputField);
        }
    }

    public void changeScene()
    {
        settingsData.sceneCheck();
    }
}
