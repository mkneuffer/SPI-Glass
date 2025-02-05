using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageData : MonoBehaviour
{
    [SerializeField] int pageNum;
    [SerializeField] GameObject page;

    public int getPageNum()
    {
        return pageNum;
    }

    public void setPageActive(bool state)
    {
        page.SetActive(state);
    }
}
