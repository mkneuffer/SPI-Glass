using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSaving : MonoBehaviour
{
    private string path = "";
    private string persistentPath = "";
    public SceneData sceneData;
    // Start is called before the first frame update
    void Awake()
    {
        
    }
    
    void Start()
    {
        setScene();
        SetPaths();
        LoadData();
        if (path == null)
        {
            setScene();
            SetPaths();
            Debug.Log("setting default scene");
        }
        else
        {
            LoadData();
            Debug.Log("loading data");
        }

        
    }

    private void SetPaths()
    {
        path = Application.dataPath + Path.AltDirectorySeparatorChar + "SceneData.json";
        persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SceneData.json";
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
        if (Input.GetKeyDown(KeyCode.P))
        {
            
            UnityEngine.SceneManagement.SceneManager.LoadScene("Test2");
            sceneData.nextScene();
            SaveData();
        }
    }

    private void setScene()
    {
        sceneData = new SceneData(0, 0);
    }

    public void SaveData()
    {
        string savePath = path;

        Debug.Log("Saving Data at " + savePath);
        string scene = JsonUtility.ToJson(sceneData);
        Debug.Log(scene);


        using StreamWriter writer = new StreamWriter(savePath);
        writer.Write(scene);
    }

    public void LoadData()
    {
        using StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();

        SceneData scene = JsonUtility.FromJson<SceneData>(json);
    }
}
