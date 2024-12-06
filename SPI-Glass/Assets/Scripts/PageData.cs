using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageData : MonoBehaviour
{
    [SerializeField] int pageNum;
    [SerializeField] GameObject page;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int getPageNum()
    {
        return pageNum;
    }

    public void setPageActive(bool state)
    {
        page.SetActive(state);
    }
}
