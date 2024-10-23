using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuItem : MonoBehaviour
{
    [HideInInspector] public Image img;
    [HideInInspector] public Transform trans;

    // Start is called before the first frame update
    void Awake()
    {
        img = GetComponent<Image>();
        trans = transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
