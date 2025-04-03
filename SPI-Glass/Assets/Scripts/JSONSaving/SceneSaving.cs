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
    public int loadScene;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject loadGameMenu;
    // Start is called before the first frame update
    void Awake()
    {
        
    }
    
    void Start()
    {
        if (mainMenu != null && loadGameMenu != null)
        {
            mainMenu.SetActive(true);
            loadGameMenu.SetActive(false);
        }
        

        
    }
    
    // Update is called once per frame

    private void setScene()
    {
        sceneData = new SceneData(0);
    }


    public void LoadNewGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scene1");
    }

    public void loadQuest2()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Scene 1");
    }

    public void changeMenu(bool change)
    {
        loadGameMenu.SetActive(change);
        mainMenu.SetActive(!change);
    }

    public void nextScene()
    {
        sceneData.nextScene();
    }


}
