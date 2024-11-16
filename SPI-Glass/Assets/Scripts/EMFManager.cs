using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.UI;

public class EMFManager : MonoBehaviour
{

    [SerializeField] private GameObject level;
    [SerializeField] private float lat1;
    [SerializeField] private float long1;
    [SerializeField] private float lat2;
    [SerializeField] private float long2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        changeLevel();
    }

    void changeLevel()
    {
        double emfLevel;
        emfLevel = Mathf.Abs(lat1 - lat2) + Mathf.Abs(long1 - long2);
        if (emfLevel <= 0)
        {
            Debug.Log("EMF failed to register");
        }
        else if (emfLevel > 0 && emfLevel <= 1)
        {
            level.GetComponent<Image>().color = Color.cyan;
        }
        else if (emfLevel > 1 && emfLevel <= 2)
        {
            level.GetComponent<Image>().color = Color.green;
        }
        else if (emfLevel > 2 && emfLevel <= 3)
        {
            level.GetComponent<Image>().color = Color.yellow;
        }
        else if (emfLevel > 3 && emfLevel <= 4)
        {
            level.GetComponent<Image>().color = Color.white;
        }
        else
        {
            level.GetComponent<Image>().color = Color.red;
        }
    }
}
