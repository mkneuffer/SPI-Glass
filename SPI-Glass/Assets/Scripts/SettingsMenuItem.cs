using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// We can add animations to the opening of menus later using DOTween
public class SettingsMenuItem : MonoBehaviour
{
    [HideInInspector] public Image img;
    [HideInInspector] public Transform trans;
    SettingsMenu settingsMenu;
    Button button;
    int index;

    //private NewBehaviourScript menuScript;

    // Awake is called to initialize variables for SettingsMenu file
    void Awake()
    {
        img = GetComponent<Image>();
        trans = transform;

        settingsMenu = trans.parent.GetComponent<SettingsMenu> ();
        index = trans.GetSiblingIndex() - 1; 

        button = GetComponent<Button> ();
        button.onClick.AddListener (OnItemClick);
    }

    void OnItemClick() {
        settingsMenu.OnItemClick (index);
    }

    void OnDestroy() {
        button.onClick.RemoveListener (OnItemClick);
    }
    
    // Update is called once per frame
    void Update()
    {
        // Probably unnecessary to keep this, but I'll leave it here just in case
    }
}
