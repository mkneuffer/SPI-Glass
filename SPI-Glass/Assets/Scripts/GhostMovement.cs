using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("SpawnGhost", 1);
    }
    int moveDirection = 1;
    int counter = 0;
    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, .05f * moveDirection, 0);
        counter++;
        if (counter % 50 == 0)
        {
            moveDirection *= -1;
        }

    }

    void MoveGhost()
    {

    }
}
