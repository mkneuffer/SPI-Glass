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
    [SerializeField] private float interactionRadius = 100f;
    [SerializeField] private float lowerScreenLimit = 100f;
    [SerializeField] private GameObject interactionDetectorPrefab;
    [SerializeField] private float maxDistance = 50f;
    [SerializeField] private float scaleSpeed = 5f;
    private int mouseClicks;
    private Vector3 worldPos;
    bool start = false;
    bool isTouch = false;
    int hold = 0;
    int flashlightHealth = 10;

    private GameObject activeDetector;
    private bool isFlashlightEnabled = false;
    private bool isHolyWaterEnabled = false;
    private bool isMenuOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        SelectFlashlight();
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
        if (isTouch != false)
        {
            HandleInteraction();
        }

        //}
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }


    private void MoveReticle()
    {
        reticle.SetActive(true);
        if (Camera.main == null || reticle == null)
        {
            Debug.Log("Error!");
            return;
        }
        Vector3 mousePos;
        if (start == false)
        {
            mousePos.x = 0;
            mousePos.y = 0;
            mousePos.z = 0;
            start = true;
        }
        else
        {
            mousePos = reticle.transform.position;
        }




        if (Input.touchCount > 1 && Input.touches[0].phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(1);
            mousePos = touch.position;
            Debug.Log("mobile input");
            isTouch = true;
            hold++;
        }
        else if (Input.mousePresent)
        {
            bool isMouseClick = Input.GetMouseButton(0);
            if (isMouseClick != false)
            {
                mousePos = Input.mousePosition;
                Debug.Log("input registered");
                isTouch = true;
                hold++;
            }
            else
            {
                //Debug.Log("No mouse input detected!");
                isTouch = false;
                hold = 0;
                mousePos.x = 100000;
                mousePos.y = 100000;
            }
        }

        //if(mousePos.y < lowerScreenLimit) {
        //    return;
        //}

        mousePos.x = Mathf.Clamp(mousePos.x, 0, Screen.width);
        mousePos.y = Mathf.Clamp(mousePos.y, lowerScreenLimit, Screen.height);


        if (float.IsInfinity(mousePos.x) || float.IsInfinity(mousePos.y))
        {
            Debug.Log("Set to infinity; resetting mouse position");
            mousePos = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        }

        float depth = Camera.main.nearClipPlane + 1f;
        mousePos.z = depth;

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

        if (reticle != null)
        {
            reticle.transform.position = mousePos;

        }
        else
        {
            Debug.Log("Error with reticle");
        }
        worldPos = worldPosition;
        //Debug.Log($"Mouse Position: {mousePos}, World Position: {worldPosition}");
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
        Collider[] hitColliders = Physics.OverlapSphere(worldPos, interactionRadius, ghostLayer);
        if (hitColliders.Length > 0)
        {
            //Debug.Log("HIT0");
            foreach (Collider hitCollider in hitColliders)
            {
                GameObject hitObject = hitCollider.gameObject;
                //Debug.Log("HIT");

                if (hitObject.CompareTag("Ghost"))
                {
                    RegisterHit(hitObject);
                    Debug.Log("Hit registered");
                }
            }
        }
    }

    private void LaunchDetector()
    {
        if (activeDetector != null)
        {
            Destroy(activeDetector);
        }
        activeDetector = Instantiate(interactionDetectorPrefab, reticle.transform.position, Quaternion.identity);
        StartCoroutine(ScaleDetector());
    }

    private IEnumerator ScaleDetector()
    {
        Vector3 initialScale = Vector3.zero;
        Vector3 maxScale = new Vector3(1f, 1f, maxDistance);

        while (activeDetector != null)
        {
            activeDetector.transform.localScale = Vector3.MoveTowards(activeDetector.transform.localScale, maxScale, scaleSpeed * Time.deltaTime);

            Collider[] hitColliders = Physics.OverlapBox(
            activeDetector.transform.position,
            activeDetector.transform.localScale / 2,
            activeDetector.transform.rotation,
            ghostLayer
        );

            foreach (Collider hitCollider in hitColliders)
            {
                GameObject hitObject = hitCollider.gameObject;
                if (hitObject.CompareTag("Ghost"))
                {
                    RegisterHit(hitObject);
                    Destroy(activeDetector);
                    yield break;
                }
            }

            if (activeDetector.transform.localScale.z >= maxDistance)
            {
                Destroy(activeDetector);
                yield break;
            }
            yield return null;
        }
    }

    private void RegisterHit(GameObject hitObject)
    {
        Debug.Log($"Hit: {hitObject.name}");
        if (isFlashlightEnabled)
        {
            Debug.Log("HIT GHOST STUNNED");
            flashlightManager.DoFlashlightDamage();
        }
        else if (isHolyWaterEnabled)
        {
            if (hold <= 1 && flashlightManager.getStun())
            {
                Debug.Log("Hit ghost damaged");
                ghostMovement.HandleHealth(1);
            }
        }
    }

    private bool IsInteractionInput()
    {
        return (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) || Input.GetMouseButtonDown(0);
    }


    private Vector3 GetInteractionPosition()
    {
        Vector3 interactionPosition;

        if (Input.mousePresent)
        {
            float depth = Camera.main.nearClipPlane + 1f;
            interactionPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, depth));
        }
        else if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            float depth = Camera.main.nearClipPlane + 1f;
            interactionPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, depth));
        }
        else
        {
            interactionPosition = reticle.transform.position;
        }

        if (float.IsInfinity(interactionPosition.x) || float.IsInfinity(interactionPosition.y))
        {
            Debug.Log("Interaction position set to infinity; resetting position");
            interactionPosition = reticle.transform.position;
        }
        return interactionPosition;
    }

    public void SelectFlashlight()
    {
        isFlashlightEnabled = true;
        isHolyWaterEnabled = false;
        Debug.Log("Flashlight selected");
        reticle.GetComponent<Image>().color = Color.yellow;
        //SetReticleVisibility(isFlashlightEnabled);
    }

    public void SelectHolyWater()
    {
        isHolyWaterEnabled = true;
        isFlashlightEnabled = false;
        Debug.Log("Holy Water selected");
        reticle.GetComponent<Image>().color = Color.cyan;
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
