using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ReticleManager2 : MonoBehaviour
{
    [SerializeField] private GameObject reticle;
    [SerializeField] private LayerMask ghostLayer;
    [SerializeField] private FlashlightHitboxManager flashlightManager;
    [SerializeField] private GhostMovement ghostMovement;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private GameObject flashlightLight;
    [SerializeField] private float interactionRadius = 100f;

    [SerializeField] private GameObject interactionDetectorPrefab;
    [SerializeField] private float maxDistance = 50f;
    [SerializeField] private float scaleSpeed = 5f;
    [SerializeField] private Button flashlightToggle;

    [SerializeField] private GameObject holyWaterPrefab;
    [SerializeField] private GameObject splashEffect;
    [SerializeField] private Transform pointThrown;
    [SerializeField] private float throwForce = 10f;

    private Vector3 worldPos;
    private bool isTouch = false;
    private int hold = 0;
    private int activeItem;

    private GameObject activeDetector;
    private bool isFlashlightEnabled = false;
    private bool isHolyWaterEnabled = false;
    private bool isMenuOpen = false;
    private bool isFlashlightHeld = false;
    private bool holyWaterCooldown = false;
    private bool hasClicked = false;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        SelectFlashlight();
        SetupListener();
        if (Camera.main == null)
        {
            Debug.LogError("Error: Main Camera is missing");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isMenuOpen)
        {
            return;
        }

        MoveReticle();

        if (isTouch && isHolyWaterEnabled)
        {
            if (!hasClicked && IsInteractionInput())
            {
                HandleInteraction();
                hasClicked = true;
            }
        }

        if (!IsInteractionInput())
        {
            hasClicked = false;
        }

        if (isFlashlightHeld)
        {
            FlashlightInteraction();
            flashlightLight.SetActive(true);
        }
        else
        {
            flashlightLight.SetActive(false);
        }
    }

    public void OnFlashlightButtonPress()
    {
        isFlashlightHeld = true;
    }

    public void OnFlashlightButtonRelease()
    {
        isFlashlightHeld = false;
    }

    private void SetupListener()
    {
        if (flashlightToggle != null)
        {
            flashlightToggle.onClick.AddListener(SelectFlashlight);
        }
        else
        {
            Debug.Log("Flashlight not assigned");
        }
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void MoveReticle()
    {
        if (Camera.main == null || reticle == null)
        {
            Debug.LogError("Error: Camera or Reticle is null");
            return;
        }

        Vector3 mousePos = Vector3.zero;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            mousePos = touch.position;
            //Debug.Log("Touch input detected");
            isTouch = true;
            //hold++;
        }
        else if (Input.mousePresent)
        {
            if (Input.GetMouseButton(0))
            {
                mousePos = Input.mousePosition;
                Debug.Log("Mouse input detected");
                isTouch = true;
                //hold++;
            }
            else
            {
                isTouch = false;
                //mousePos = new Vector3(100000, 100000, 0);
                //hold = 0;
            }
        }
        else
        {
            isTouch = false;
            return;
        }

        //mousePos.x = Mathf.Clamp(mousePos.x, 0, Screen.width);
        //mousePos.y = Mathf.Clamp(mousePos.y, lowerScreenLimit, Screen.height);

        float depth = Camera.main.nearClipPlane + 1f;
        mousePos.z = depth;

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

        if (reticle != null)
        {
            reticle.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0); ;
        }
        else
        {
            Debug.LogError("Error: Reticle is null");
        }

        worldPos = worldPosition;
    }

    public void FlashlightInteraction()
    {
        if (isFlashlightEnabled)
        {
            //Debug.Log("Flashlight triggered");
            HandleInteraction();
        }
    }

    private void HandleInteraction()
    {
        if (reticle == null)
        {
            Debug.LogError("Error: Reticle is null");
            return;
        }

        if (!IsInteractionInput())
        {
            LaunchDetector();
        }

        Collider[] hitColliders = Physics.OverlapSphere(worldPos, interactionRadius, ghostLayer);
        foreach (Collider hitCollider in hitColliders)
        {
            GameObject hitObject = hitCollider.gameObject;
            if (hitObject.CompareTag("Ghost"))
            {
                RegisterHit(hitObject);
                //Debug.Log("Hit registered");
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
        //Debug.Log($"Hit: {hitObject.name}");
        if (isFlashlightEnabled == true)
        {
            Debug.Log("Hit ghost with flashlight");
            flashlightManager.DoFlashlightDamage();
        }
        else if (isHolyWaterEnabled == true && !holyWaterCooldown)
        {
            holyWaterCooldown = true;
            ghostMovement.HandleHealth(1);
            StartCoroutine(ThrowHolyWater(hitObject.transform.position));
            StartCoroutine(HolyWaterCooldown());
            if (hold <= 1 && flashlightManager.getStun() == true)
            {
                Debug.Log("Hit ghost damaged 2");
            }
        }
    }

    private bool IsInteractionInput()
    {
        return (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0);
    }

    public void SelectFlashlight()
    {
        isFlashlightEnabled = true;
        isHolyWaterEnabled = false;
        Debug.Log("Flashlight selected");
        if (flashlightToggle != null)
        {
            flashlightToggle.gameObject.SetActive(true);
            flashlightToggle.interactable = true;
        }

        reticle.GetComponent<Image>().color = Color.yellow;
    }

    public void SelectHolyWater()
    {
        isHolyWaterEnabled = true;
        isFlashlightEnabled = false;
        Debug.Log("Holy Water selected");

        if (flashlightToggle != null)
        {
            flashlightToggle.gameObject.SetActive(false);
            flashlightToggle.interactable = false;
        }

        reticle.GetComponent<Image>().color = Color.cyan;
    }
    private IEnumerator HolyWaterCooldown()
    {
        yield return new WaitForSeconds(0.2f);
        holyWaterCooldown = false;
    }

    private IEnumerator ThrowHolyWater(Vector3 targetPosition)
    {

        GameObject holyWater = Instantiate(holyWaterPrefab, pointThrown.position, Quaternion.identity);
        //HolyWaterHitboxManager holyWaterHitbox = holyWater.GetComponent<HolyWaterHitboxManager>();
        // if(holyWaterHitbox != null)
        //  {
        //     holyWaterHitbox.ghostMovement = ghostMovement;
        //holyWaterHitbox.Initialize(targetPosition);
        //  }

        Rigidbody rb = holyWater.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = pointThrown.forward;
            rb.AddForce(direction * throwForce, ForceMode.Impulse);
            Debug.Log($"Spawn Position: {pointThrown.position}");
        }

        Vector3 startPosition = holyWater.transform.position;
        float maxDistance = 10f;
        while (holyWater != null)
        {
            if (Vector3.Distance(startPosition, holyWater.transform.position) > maxDistance)
            {
                Destroy(holyWater);
                SpawnHolyWater();
                yield break;
            }

            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ghost"))
        {
            Vector3 effectPosition = collision.contacts[0].point;
            GameObject splash = Instantiate(splashEffect, effectPosition, Quaternion.identity);

            ParticleSystem ps = splash.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(splash, ps.main.duration);
            }
            else
            {
                Destroy(splash, 0.5f);
            }
            Destroy(gameObject);
        }
    }

    private void SpawnHolyWater()
    {
        Instantiate(holyWaterPrefab, pointThrown.position, Quaternion.identity);
    }

    public void ToggleMenu(bool isOpen)
    {
        isMenuOpen = isOpen;
    }

    public void checkCurrentItem()
    {
        if (inventoryManager != null)
        {
            if (inventoryManager.getCurrentItemNum() != activeItem)
            {
                activeItem = inventoryManager.getCurrentItemNum();
                if (activeItem == 0)
                {
                    SelectFlashlight();
                }
                else if (activeItem == 1)
                {
                    SelectHolyWater();
                }
            }
        }
    }
}
