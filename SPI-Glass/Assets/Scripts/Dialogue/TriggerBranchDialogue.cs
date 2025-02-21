using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBranchDialogue : MonoBehaviour
{
    public TextAsset inkJSONDialogue1;
    public TextAsset inkJSONDialogue2;
    public bool state = false;
    public int dialogue;


    void Start()
    {
        dialogue = PlayerPrefs.GetInt("DialogueState", 0); // Default
    }


    // Update is called once per frame
    void Update()
    {
        if (state == false)
        {
            if (dialogue == 1)
            {
                DialogueManager.GetInstance().EnterDialogueMode(inkJSONDialogue1, true);
            }
            else if (dialogue == 2)
            {
                DialogueManager.GetInstance().EnterDialogueMode(inkJSONDialogue2, true);
            }
            else
            {
                Debug.Log("no dialogue available");
            }
            state = true;
        }
    }
}
