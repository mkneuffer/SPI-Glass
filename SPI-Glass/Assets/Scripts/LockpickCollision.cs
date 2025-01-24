using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockpickCollision : MonoBehaviour
{
    [SerializeField] GameObject PinCollider;
    [SerializeField] GameObject Lockpick;
    //[SerializeField] GameObject Lockpick;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Lockpick)
        {
            Vector3 position = this.transform.position;
            position.y++;
            Debug.Log("Pick has collided");
        }
    }
/*
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == Lockpick)
        {
            Debug.Log("Player exited the collider area.");  // Debug statement for testing
            //emfManager.setEMFActive(false);
        }
    } */
}
