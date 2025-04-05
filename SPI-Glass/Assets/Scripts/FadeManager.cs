using System.Collections;
using UnityEngine;

public class HideAfterDelay : MonoBehaviour
{
    public GameObject objectToHide;
    public float delayInSeconds = 15f;

    void Start()
    {
        if (objectToHide != null)
        {
            StartCoroutine(HideAfterSeconds(objectToHide, delayInSeconds));
        }
    }

    IEnumerator HideAfterSeconds(GameObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        obj.SetActive(false);
    }
}
