using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReticleManager : MonoBehaviour
{
    [SerializeField] private GameObject reticle; 
    [SerializeField] private LayerMask ghostLayer; 
    [SerializeField] private FlashlightHitboxManager flashlightManager; 
    [SerializeField] private GhostMovement ghostMovement;
    private bool isFlashlightEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        MoveReticle();
        HandleInteraction();
    }

    private void MoveReticle() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        reticle.transform.position = mousePos;
    }

    private void HandleInteraction()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ghostLayer))
        {
            if (hit.collider.CompareTag("Ghost"))
            {
                if (isFlashlightEnabled)
                {
                    flashlightManager.StunGhost();
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    ghostMovement.HandleHealth(1);
                }
            }
        }
    }

    public void SelectFlashlight(bool isActive) {
        isFlashlightEnabled = isActive;
    }

}
