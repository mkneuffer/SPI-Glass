using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour
{
    public int maximum;
    public int current = 0;
    public Image mask;
    public SemanticQuery semanticQuery;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentFill();
    }

    void GetCurrentFill()
    {
        current = semanticQuery.woodInProgressBar;
        maximum = semanticQuery.woodNeededToCraftGrail;
        float fillAmount = (float)current / (float)maximum;
        mask.fillAmount = fillAmount;
    }
}
