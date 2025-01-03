using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{

    [SerializeField] private GameObject square1;
    [SerializeField] private GameObject square2;
    [SerializeField] private GameObject square3;
    [SerializeField] private GameObject puzzleCanvas;
    [SerializeField] private DialogueManager dialogueManager;
    public TextAsset inkJSON;
    public bool puzzleSolved = false;
    private int zValue1;
    private int zValue2;
    private int zValue3;

    // Start is called before the first frame update
    void Start()
    {
        zValue1 = Random.Range(1, 7) * 45;
        zValue2 = Random.Range(1, 7) * 45;
        zValue3 = Random.Range(1, 7) * 45;
        square1.transform.Rotate(0, 0, zValue1);
        square2.transform.Rotate(0, 0, zValue2);
        square3.transform.Rotate(0, 0, zValue3);
        puzzleCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (puzzleSolved == true)
        {
            continueHolyWater();
            puzzleSolved = false;
            
        }
    }

    void checkSolved()
    {
        if (zValue1 == 0 && zValue2 == 0 && zValue3 == 0) 
        {
            puzzleSolved = true;
            //SceneManager.LoadScene(6);
        }
    }

    void continueHolyWater()
    {
        setCanvasState(false);
        dialogueManager.EnterDialogueMode(inkJSON, true);
        // holy water get
    }

    public void rotateSquare(int i)
    {
        if (i == 1)
        {
            square1.transform.Rotate(0, 0, 45);

            if (zValue1 >= 315)
            {
                zValue1 = 0;
            }
            else
            {
                zValue1 += 45;
            }
        }
        else if (i == 2)
        {
            square2.transform.Rotate(0, 0, 45);

            if (zValue2 >= 315)
            {
                zValue2 = 0;
            }
            else
            {
                zValue2 += 45;
            }
        }
        else
        {
            square3.transform.Rotate(0, 0, 45);

            if (zValue3 >= 315)
            {
                zValue3 = 0;
            }
            else
            {
                zValue3 += 45;
            }
        }
        checkSolved();
    }

    public void setCanvasState (bool state)
    {
        puzzleCanvas.SetActive(state);
    }
}
