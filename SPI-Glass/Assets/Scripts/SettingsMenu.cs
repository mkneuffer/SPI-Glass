using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    [Header("space between icons")]
    [SerializeField] Vector2 spacing;

    Button Settings;
    SettingsMenuItem[] menuItems;
    bool isExpanded = false;

    Vector2 SettingsPosition;
    int itemsCount;

    // Start is called before the first frame update
    void Start()
    {
        itemsCount = transform.childCount - 1;
        menuItems = new SettingsMenuItem[itemsCount];
        for(int i = 0; i < itemsCount; i++) {
            menuItems[i] = transform.GetChild (i+1).GetComponent <SettingsMenuItem>();
        }

        Settings = transform.GetChild (0).GetComponent <Button>();
        Settings.onClick.AddListener(ToggleMenu);
        Settings.transform.SetAsLastSibling();

        SettingsPosition = Settings.transform.position;

        // Resets all icons to Settings menu button
        ResetPositions();
    }

    void ResetPositions() {
        for(int i = 0; i < itemsCount; i++) {
            menuItems[i].trans.position = SettingsPosition;
        }
    }

    void ToggleMenu() {
        isExpanded = !isExpanded;

        if(isExpanded) {
            for(int i = 0; i < itemsCount; i++) {
                menuItems[i].trans.position = SettingsPosition + spacing * (i+1);
            }
        } else {
            for(int i = 0; i < itemsCount; i++) {
                menuItems[i].trans.position = SettingsPosition;
            }
        }
    }
/*
    public void OnItemClick(int index) {
        // Does not include the main settings button
        switch(index) {
            case 0:
                Debug.log("Audio");
                break;
            case 1:
                Debug.log("Vibration");
                break;
        }
    }

    // Update is called once per frame
    void OnDestroy()
    {
        Settings.onClick.RemoveListener(ToggleMenu);
    }
    */
}
