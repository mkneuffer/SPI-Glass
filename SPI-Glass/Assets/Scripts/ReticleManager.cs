using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ReticleManager : MonoBehaviour
{
    [SerializeField] private GameObject reticle; 
    [SerializeField] private LayerMask ghostLayer; 
    [SerializeField] private FlashlightHitboxManager flashlightManager; 
    [SerializeField] private GhostMovement ghostMovement;
    [SerializeField] private float raycastDistance = 50f;
    [SerializeField] private float lowerScreenLimit = 200f;
    [SerializeField] private float interactionRadius = 1f;
    private bool isFlashlightEnabled = false;
    private bool isHolyWaterEnabled = false;
    private bool isMenuOpen = false;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        //SetReticleVisibility(false);

        if(Camera.main == null) {
            Debug.Log("Error with camera");
        }
    }

    // Update is called once per frame
    void Update()
    {

        if(isMenuOpen) {
            //SetReticleVisibility(false);
            return;
        }

        if(!IsPointerOverUI()) {
            MoveReticle();
            HandleInteraction();
        }

    }

    private bool IsPointerOverUI() {
        return EventSystem.current.IsPointerOverGameObject();
    }


    private void MoveReticle() {
        if(Camera.main == null || reticle == null) {
            Debug.Log("Error!");
            return;
        }

        Vector3 mousePos = Input.mousePosition;
        if(mousePos.y < lowerScreenLimit) {
            return;
        }

        mousePos.x = Mathf.Clamp(mousePos.x, 0, Screen.width);
        mousePos.y = Mathf.Clamp(mousePos.y, lowerScreenLimit, Screen.height);
        mousePos.z = 10f;
        //Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

        if(reticle != null) {
            reticle.transform.position = mousePos;
        } else {
            Debug.Log("Error with reticle");
        }
    }

    private void HandleInteraction()
    {
        if(reticle == null) {
            Debug.Log("Error!");
            return;
        }
        Vector3 reticleWorldPos = Camera.main.ScreenToWorldPoint(reticle.transform.position);
        reticleWorldPos.z = -3f;

        Collider[] hitColliders = Physics.OverlapSphere(reticleWorldPos, interactionRadius, ghostLayer);

        if (hitColliders.Length > 0)
        {
            foreach (Collider hitCollider in hitColliders)
            {
                GameObject hitObject = hitCollider.gameObject;

                if (hitObject.CompareTag("Ghost"))
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
    }

    public void SelectFlashlight(bool isActive) {
        isFlashlightEnabled = isActive;
        isHolyWaterEnabled = false;
    }

    public void SelectHolyWater(bool isActive) {
        isHolyWaterEnabled = isActive;
        isFlashlightEnabled = false;
    }

    private void SetReticleVisibility(bool isVisible) {
        if(reticle != null) {
            reticle.SetActive(isVisible);
        }
    }

    public void ToggleMenu(bool isOpen) {
        isMenuOpen = isOpen;
    }

}
