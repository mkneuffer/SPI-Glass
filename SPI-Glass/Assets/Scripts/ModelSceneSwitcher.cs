using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcherOnClick : MonoBehaviour
{
    void OnMouseDown()
    {
        Debug.Log("Object clicked!");
        SceneManager.LoadScene(2);
    }
}
