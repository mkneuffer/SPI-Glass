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
    [SerializeField] private float interactionRadius = 1f;
    [SerializeField] private float lowerScreenLimit = 200f;
    [SerializeField] private GameObject interactionDetectorPrefab;
    [SerializeField] private float maxDistance =  50f;
    [SerializeField] private float scaleSpeed = 5f;

    private GameObject activeDetector;
    private bool isFlashlightEnabled = false;
    private bool isHolyWaterEnabled = false;
    private bool isMenuOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        //SetReticleVisibility(false);

        if (Camera.main == null)
        {
            Debug.Log("Error with camera");
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (isMenuOpen)
        {
            //SetReticleVisibility(false);
            return;
        }

        //if (!IsPointerOverUI())
        //{
        MoveReticle();
        HandleInteraction();
        //}
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }


    private void MoveReticle()
    {
        if (Camera.main == null || reticle == null)
        {
            Debug.Log("Error!");
            return;
        }

        Vector3 mousePos = Input.mousePosition;
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);
            mousePos = touch.position;
        }
        if (mousePos.y < lowerScreenLimit)
        {
            return;
        }
        mousePos.x = Mathf.Clamp(mousePos.x, 0, Screen.width);
        mousePos.y = Mathf.Clamp(mousePos.y, lowerScreenLimit, Screen.height);
        mousePos.z = 10f;
        //Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

        if (reticle != null)
        {
            reticle.transform.position = mousePos;
        }
        else
        {
            Debug.Log("Error with reticle");
        }
        Vector3 reticleWorldPos = Camera.main.ScreenToWorldPoint(reticle.transform.position);
        reticleWorldPos.z = 0f;
    }

    private void HandleInteraction()
    {
        if (reticle == null)
        {
            Debug.Log("Error!");
            return;
        }

        if (!IsInteractionInput())
        {
            LaunchDetector();
        }

        Vector3 interactionPosition = GetInteractionPosition();
        Collider[] hitColliders = Physics.OverlapSphere(reticle.transform.position, interactionRadius, ghostLayer);
        if (hitColliders.Length > 0)
        {
            Debug.Log("HIT1");
            foreach (Collider hitCollider in hitColliders)
            {
                GameObject hitObject = hitCollider.gameObject;
                Debug.Log("HIT");

                if (hitObject.CompareTag("Ghost"))
                {
                    RegisterHit(hitObject);
                }
            }
        }
    }

    private void LaunchDetector() {
        if(activeDetector != null) {
            Destroy(activeDetector);
        }
        activeDetector = Instantiate(interactionDetectorPrefab, reticle.transform.position, Quaternion.identity);
        StartCoroutine(ScaleDetector());
    }

    private IEnumerator ScaleDetector() {
        Vector3 initialScale = Vector3.zero;
        Vector3 maxScale = new Vector3(1f, 1f, maxDistance);

        while(activeDetector != null) {
            activeDetector.transform.localScale = Vector3.MoveTowards(activeDetector.transform.localScale, maxScale, scaleSpeed * Time.deltaTime);

            Collider[] hitColliders = Physics.OverlapBox(
            activeDetector.transform.position, 
            activeDetector.transform.localScale / 2, 
            activeDetector.transform.rotation, 
            ghostLayer
        );

            foreach(Collider hitCollider in hitColliders) {
                GameObject hitObject = hitCollider.gameObject;
                if(hitObject.CompareTag("Ghost")) {
                    RegisterHit(hitObject);
                    Destroy(activeDetector);
                    yield break;
                }
            }

            if(activeDetector.transform.localScale.z >= maxDistance) {
                Destroy(activeDetector);
                yield break;
            }
            yield return null;
        }
    }

    private void RegisterHit(GameObject hitObject) {
        Debug.Log($"Hit: {hitObject.name}");
        if (isFlashlightEnabled)
        {
            Debug.Log("HIT GHOST STUNNED");
            flashlightManager.StunGhost();
        }
        else if (isHolyWaterEnabled)
        {
            ghostMovement.HandleHealth(1);
        } else {
            return;
        }
}

    private bool IsInteractionInput() {
        return(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) || Input.GetMouseButtonDown(0);
    }

    private Vector3 GetInteractionPosition() {
        Vector3 interactionPosition;
        if(Input.mousePresent) {
            interactionPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            interactionPosition.z = 0f;
        } else if(Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            interactionPosition = Camera.main.ScreenToWorldPoint(touch.position);
            interactionPosition.z = 0f;
        } else {
            interactionPosition = reticle.transform.position;
        }
        return interactionPosition;
    }

    public void SelectFlashlight(bool isActive)
    {
        isFlashlightEnabled = isActive;
        isHolyWaterEnabled = false;
        //SetReticleVisibility(isFlashlightEnabled);
    }

    public void SelectHolyWater(bool isActive)
    {
        isHolyWaterEnabled = isActive;
        isFlashlightEnabled = false;
        //SetReticleVisibility(isHolyWaterEnabled);
    }
    /*
        private void SetReticleVisibility(bool isVisible) {
            if(reticle != null) {
                reticle.SetActive(isVisible);
            }
        }
    */
    public void ToggleMenu(bool isOpen)
    {
        isMenuOpen = isOpen;
        //SetReticleVisibility(false);
    }

}
