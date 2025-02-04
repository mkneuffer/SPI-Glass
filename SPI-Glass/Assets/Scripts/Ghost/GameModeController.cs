using UnityEngine;
using UnityEngine.UI;

public class GameModeController : MonoBehaviour
{
    public Launcher launcher;
    public Light flashlight;
    public Button launcherModeButton;
    public Button flashlightModeButton;

    [Header("Flashlight Visibility Object")]
    public GameObject objectToHide; // Assign in Inspector

    private enum Mode { None, Launcher, Flashlight }
    private Mode currentMode = Mode.None;

    void Start()
    {
        launcherModeButton.onClick.AddListener(SetLauncherMode);
        flashlightModeButton.onClick.AddListener(SetFlashlightMode);

        SetNoneMode(); // Start in None mode
    }

    private void SetLauncherMode()
    {
        currentMode = Mode.Launcher;
        launcher.enabled = true;
        flashlight.enabled = false;
        UpdateObjectVisibility(false);
        Debug.Log("Switched to Launcher mode.");
    }

    private void SetFlashlightMode()
    {
        currentMode = Mode.Flashlight;
        launcher.enabled = false;
        flashlight.enabled = false;
        UpdateObjectVisibility(false);
        Debug.Log("Switched to Flashlight mode.");
    }

    private void SetNoneMode()
    {
        currentMode = Mode.None;
        launcher.enabled = false;
        flashlight.enabled = false;
        UpdateObjectVisibility(false);
        Debug.Log("Starting in None mode. No flashlight or launcher enabled.");
    }

    void Update()
    {
        if (currentMode == Mode.Flashlight)
        {
            if (Input.GetMouseButtonDown(0))
            {
                flashlight.enabled = true;
                UpdateObjectVisibility(true);
                Debug.Log("Flashlight turned ON.");
            }

            if (Input.GetMouseButtonUp(0))
            {
                flashlight.enabled = false;
                UpdateObjectVisibility(false);
                Debug.Log("Flashlight turned OFF.");
            }

            if (flashlight.enabled)
            {
                PerformFlashlightRaycast();
            }
        }
    }

    private void PerformFlashlightRaycast()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        foreach (RaycastHit hit in hits)
        {
            GhostHealth ghost = hit.collider.GetComponentInParent<GhostHealth>();

            if (ghost != null)
            {
                if (!ghost.IsStunned()) 
                {
                    Debug.Log($"Flashlight hit: {hit.collider.gameObject.name}");
                    ghost.HandleFlashlight();
                }
                return;
            }
        }
    }

    private void UpdateObjectVisibility(bool isVisible)
    {
        if (objectToHide != null)
        {
            objectToHide.SetActive(isVisible && currentMode == Mode.Flashlight);
        }
    }
}
