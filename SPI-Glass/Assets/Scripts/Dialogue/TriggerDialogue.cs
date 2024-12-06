using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTriggerDialogue : MonoBehaviour
{
    public TextAsset inkJSONDialogue;
    public TextAsset inkJSONPrompt;
    public bool state = false;
    public bool onStart = true;

    // Update is called once per frame
    void Update()
    {
        if (state == false && onStart == true)
        {
            if (inkJSONDialogue != null)
            {
                DialogueManager.GetInstance().EnterDialogueMode(inkJSONDialogue, true);
                state = true;
            }
            else if (inkJSONPrompt != null)
            {
                DialogueManager.GetInstance().EnterAutomaticDialogueMode(inkJSONPrompt, false, 0.6f);
                state = true;
            }
            
        }
    }

    public void callDialogue(TextAsset inkJSON) 
    {
        DialogueManager.GetInstance().EnterDialogueMode(inkJSON, true);
        state = true;
    }
}
