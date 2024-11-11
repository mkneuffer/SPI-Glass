using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowUIPanelOnEnter : MonoBehaviour
{
    public GameObject player;
    public GameObject uiPanel;

    private SphereCollider sphereCollider;

    private void Start()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }

        sphereCollider = gameObject.GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;  // Ensure the collider is set to be a trigger
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            Debug.Log("Player entered the collider area.");  // Debug statement for testing
            if (uiPanel != null)
            {
                uiPanel.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            Debug.Log("Player exited the collider area.");  // Debug statement for testing
            if (uiPanel != null)
            {
                uiPanel.SetActive(false);
            }
        }
    }
}
