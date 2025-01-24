using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompendiumManager : MonoBehaviour
{
    [SerializeField] private PageData[] pages;
    [SerializeField] private int currentPage;
    // Start is called before the first frame update
    void Start()
    {
        goToPage(0);
        currentPage = 0;
    }

    public void nextPage()
    {
        if ((currentPage + 1) < pages.Length)
        {
            currentPage++;
            swapPage();
        }
    }

    public void prevPage()
    {
        if ((currentPage - 1) >= 0)
        {
            currentPage--;
            swapPage();
        }
    }

    public void swapPage()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (currentPage == pages[i].getPageNum())
            {
                pages[i].setPageActive(true);
            }
            else
            {
                pages[i].setPageActive(false);
            }
        }
    }

    public void goToPage(int pageNum)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (pageNum == pages[i].getPageNum())
            {
                pages[i].setPageActive(true);
            }
            else
            {
                pages[i].setPageActive(false);
            }
        }
    }

    public void returnToMap()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
