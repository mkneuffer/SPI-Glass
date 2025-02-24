using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{

    string scene;
    public TMPro.TMP_Dropdown dropdown;
    public int dropdownValue;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        dropdownValue = dropdown.value;
    }

    public void setScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(dropdownValue + 1);
    }

    public void changeScene()
    {
        
    }
}
