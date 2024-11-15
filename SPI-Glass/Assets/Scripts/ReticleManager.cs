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
        reticle.transform.position = mousePos;
    }

    void HandleInteraction() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ghostLayer)) {
            if (isFlashlightEnabled) {
                flashlightManager.StunGhost();
            }
            else if (Input.GetMouseButtonDown(0)) {
                ghostMovement.HandleHealth(1);
            }
        }
    }

    private void CheckFlashlightInteraction() {
        Ray ray = Camera.main.ScreenPointToRay(reticle.transform.position);
        if(Physics.Raycast(ray, out RaycastHit hit)) {
            if(hit.collider.CompareTag("Ghost")) {
                flashlightManager.StunGhost();
            }
        }
    }

        private void CheckHolyWaterInteraction() {
        Ray ray = Camera.main.ScreenPointToRay(reticle.transform.position);
        if(Physics.Raycast(ray, out RaycastHit hit)) {
            if(hit.collider.CompareTag("Ghost")) {
                ghostMovement.HandleHealth(1);
            }
        }
    }

    public void SelectFlashlight(bool isActive) {
        isFlashlightEnabled = isActive;
    }

}
