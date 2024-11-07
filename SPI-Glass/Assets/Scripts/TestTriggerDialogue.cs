using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTriggerDialogue : MonoBehaviour
{
    public TextAsset inkJSONChoice;
    public TextAsset inkJSONNoChoice;

    // Update is called once per frame
    void Update()
    {
        bool clicked = Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began);
        if (clicked)
        {
            DialogueManager.GetInstance().EnterDialogueMode(inkJSONChoice, true);
        }
        else if (Input.GetMouseButtonDown(1) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began))
        {
            DialogueManager.GetInstance().EnterDialogueMode(inkJSONNoChoice, false);
        }
    }
}
