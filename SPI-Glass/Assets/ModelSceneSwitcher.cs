using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcherOnClick : MonoBehaviour
{
    [Tooltip("Name of the scene to switch to when the object is clicked")]
    public string SceneName;

    void OnMouseDown()
    {
        // Check if the SceneName is not empty
        if (!string.IsNullOrEmpty(SceneName))
        {
            SceneManager.LoadScene(SceneName);
        }
        else
        {
            Debug.LogWarning("SceneName is not set. Please set it in the Inspector.");
        }
    }
}
