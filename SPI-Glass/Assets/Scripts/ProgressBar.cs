using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//This script can be used to create any progress bar
[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour
{
    public int maximum;
    public int current = 0;
    public Image mask;
    public SemanticQuery semanticQuery;
    [SerializeField] bool testing;

    // Start is called before the first frame update
    void Start()
    {
        testing = false;
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentFill();
    }

    void GetCurrentFill()
    {
        if (!testing)
        {
            current = semanticQuery.woodInProgressBar;
            maximum = semanticQuery.woodNeededToCraftGrail;
        }
        float fillAmount = (float)current / (float)maximum;
        mask.fillAmount = fillAmount;
    }
}
