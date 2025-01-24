using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockpickPuzzle : MonoBehaviour
{
    //public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W)) {
            Vector3 position = this.transform.position;
            position.y++;
            this.transform.position = position;
            //Debug.Log("W Key is Pressed!");
        }

        if(Input.GetKey(KeyCode.S)) {
            Vector3 position = this.transform.position;
            position.y--;
            this.transform.position = position;
            //Debug.Log("S Key is Pressed!");
        }

        if(Input.GetKey(KeyCode.A)) {
            Vector3 position = this.transform.position;
            position.x--;
            this.transform.position = position;
            //Debug.Log("A Key is Pressed!");
        }

        if(Input.GetKey(KeyCode.D)) {
            Vector3 position = this.transform.position;
            position.x++;
            this.transform.position = position;
            //Debug.Log("D Key is Pressed!");
        }
    }
}
