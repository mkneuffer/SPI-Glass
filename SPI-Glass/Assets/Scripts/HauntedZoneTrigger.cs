using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowUIPanelOnEnter : MonoBehaviour
{
    public GameObject player;
    public GameObject uiPanel;
    [SerializeField] private EMFManager emfManager;

    private SphereCollider sphereCollider;

    private void Start()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }

        sphereCollider = gameObject.GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;  // Ensure the collider is set to be a trigger
        emfManager.setEMFActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            Debug.Log("Player entered the collider area.");  // Debug statement for testing
            emfManager.setEMFActive(true);
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
            emfManager.setEMFActive(false);
            if (uiPanel != null)
            {
                uiPanel.SetActive(false);
            }
        }
    }
}
