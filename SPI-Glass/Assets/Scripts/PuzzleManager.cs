using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{

    [SerializeField] private GameObject square1;
    [SerializeField] private GameObject square2;
    [SerializeField] private GameObject square3;
    public bool puzzleSolved = false;
    public int zValue1;
    public int zValue2;
    public int zValue3;
    // Start is called before the first frame update
    void Start()
    {
        zValue1 = Random.Range(1, 7) * 45;
        zValue2 = Random.Range(1, 7) * 45;
        zValue3 = Random.Range(1, 7) * 45;
        square1.transform.Rotate(0, 0, zValue1);
        square2.transform.Rotate(0, 0, zValue2);
        square3.transform.Rotate(0, 0, zValue3);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (puzzleSolved == true)
        {
            continueHolyWater();
        }
    }

    void checkSolved()
    {
        if (zValue1 == 0 && zValue2 == 0 && zValue3 == 0) 
        {
            puzzleSolved = true;
        }
    }

    void continueHolyWater ()
    {
        // call dialogue
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
}