using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneSwitcherOnClick : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 3f;

    void OnMouseDown()
    {
        StartCoroutine(LoadScene3());
        
        
    }

    IEnumerator LoadScene3()
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(2);
    }
}
