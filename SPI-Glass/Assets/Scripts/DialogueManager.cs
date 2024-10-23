using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public Dialogue dialogueBox;
    public Canvas parentCanvas;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        string[] lines = { "test1", "test2", "test3" };
        if (Input.GetMouseButtonDown(1))
        {
            DisplayTextBox(lines);
        }
    }

    void DisplayTextBox(string[] lines)
    {

        Instantiate(dialogueBox, parentCanvas.transform);
        foreach (string line in lines)
        {
            dialogueBox.AddText(line);
        }
    }
}
