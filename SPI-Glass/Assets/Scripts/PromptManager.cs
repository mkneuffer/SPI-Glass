using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptManager : MonoBehaviour
{
    [SerializeField] private GameObject prompt;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        prompt.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 5)
        {
            prompt.SetActive(false);
        }
    }
}
