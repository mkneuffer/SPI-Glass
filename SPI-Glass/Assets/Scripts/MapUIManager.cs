using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUIManager : MonoBehaviour
{

    [SerializeField] Image settingsButton;
    [SerializeField] GameObject expandMenu;
    // Start is called before the first frame update
    void Start()
    {
        expandMenu.SetActive(false);
    }

    public void openMenu()
    {
        expandMenu.SetActive(true);
    }

    public void closeMenu()
    {
        expandMenu.SetActive(false);
    }

    public void toggleMenu()
    {
        if (expandMenu.activeSelf == true)
        {
            expandMenu.SetActive(false);
        }
        else
        {
            expandMenu.SetActive(true);
        }
    }
}
