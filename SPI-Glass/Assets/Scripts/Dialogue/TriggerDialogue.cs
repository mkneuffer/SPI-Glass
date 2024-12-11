using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTriggerDialogue : MonoBehaviour
{
    public TextAsset inkJSONDialogue;
    public TextAsset inkJSONPrompt;
    public bool state = false;

    // Update is called once per frame
    void Update()
    {
        if (state == false)
        {
            if (inkJSONDialogue != null)
            {
                DialogueManager.GetInstance().EnterDialogueMode(inkJSONDialogue, true);
            }
            else
            {
                DialogueManager.GetInstance().EnterAutomaticDialogueMode(inkJSONPrompt, false, 0.6f);
            }
            state = true;
        }
    }
}
