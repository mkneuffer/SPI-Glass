using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    List<string> lines;
    public float textSpeed;
    private int index;

    // Start is called before the first frame update
    void Start()
    {
        lines = new List<string>();
        textComponent.text = string.Empty;
        index = 0;
    }

    void Awake()
    {
        StartCoroutine(TypeLine());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    public void AddText(string text)
    {
        lines.Add(text);
        Debug.Log("Adding: " + text);
        Debug.Log("linescounr addtext:" + lines.Count);
    }


    IEnumerator TypeLine()
    {

        Debug.Log("index:" + index);
        Debug.Log("linescounr typeline:" + lines.Count);

        Debug.Log("lines[0]" + lines[0]);
        foreach (char c in lines[index].ToCharArray())
        {
            Debug.Log(c);
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Count - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            lines.Clear();
            Destroy(gameObject);
        }
    }
}
