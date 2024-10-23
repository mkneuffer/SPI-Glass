using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTriggerDialogue : MonoBehaviour
{
    public TextAsset inkJSON;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !DialogueManager.GetInstance().isDialoguePlaying)
        {
            Debug.Log("calling dialogue");
            DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
        }
    }
}
