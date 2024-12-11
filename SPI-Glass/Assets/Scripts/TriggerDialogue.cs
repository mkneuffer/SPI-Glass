using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialogue : MonoBehaviour
{
    public TextAsset inkJSON;
    public bool state = false;

    // Update is called once per frame
    void Update()
    {
        if (state == false)
        {
            DialogueManager.GetInstance().EnterDialogueMode(inkJSON, true);
            state = true;
        }
    }
}
