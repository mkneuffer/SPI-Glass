using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class RopeScript : MonoBehaviour
{
    [SerializeField] GameObject ropeHead;
    [SerializeField] GameObject ropeBody;
    [SerializeField] int length = 10;
    List<GameObject> ropeParts = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        ropeParts.Add(ropeHead);
        for (int i = 0; i < length; i++)
        {
            GameObject instantiatedRope = Instantiate(ropeBody, transform);
            if (i == 0)
                instantiatedRope.transform.position = ropeParts[i].transform.position - new Vector3(0, 0.5f, 0);
            else
                instantiatedRope.transform.position = ropeParts[i].transform.position - new Vector3(0, 1, 0);
            HingeJoint joint = instantiatedRope.GetComponent<HingeJoint>();
            if (joint != null)
            {
                joint.connectedBody = ropeParts[i].GetComponent<Rigidbody>();
                joint.autoConfigureConnectedAnchor = true;
            }
            ropeParts.Add(instantiatedRope);
        }
    }
}
