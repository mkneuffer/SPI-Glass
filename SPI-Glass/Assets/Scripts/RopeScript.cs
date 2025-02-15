using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class RopeScript : MonoBehaviour
{
    [SerializeField] GameObject ropeHead;
    [SerializeField] GameObject ropeBody;
    [SerializeField] int length = 10;
    [SerializeField] float force = 1000f;
    [SerializeField] bool gravity = false;
    List<GameObject> ropeParts = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        ropeParts.Add(ropeHead);
        for (int i = 0; i < length; i++)
        {
            GameObject instantiatedRope = Instantiate(ropeBody, transform);
            if (i == 0)
                instantiatedRope.transform.position = ropeParts[i].transform.position - new Vector3(0, ropeBody.transform.localScale.y, 0);
            else
                instantiatedRope.transform.position = ropeParts[i].transform.position - new Vector3(0, ropeBody.transform.localScale.y * 2, 0);
            HingeJoint joint = instantiatedRope.GetComponent<HingeJoint>();
            if (joint != null)
            {
                joint.connectedBody = ropeParts[i].GetComponent<Rigidbody>();
                joint.autoConfigureConnectedAnchor = true;
            }
            ropeParts.Add(instantiatedRope);
        }
        transform.Rotate(new Vector3(90, 0, 0));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            for (int i = 1; i < ropeParts.Count; i++)
            {
                //ropeParts[i].GetComponent<Rigidbody>().useGravity = true;
            }

            Debug.Log("force applied");
            Rigidbody rigidBody = ropeParts[ropeParts.Count - 1].GetComponent<Rigidbody>();
            //rigidBody.AddForce(new Vector3(force, 0, 0));
            GetComponent<Rigidbody>().AddForce(new Vector3(force, force, 0));
            GetComponent<Rigidbody>().useGravity = gravity;
        }
    }
}
