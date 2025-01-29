using UnityEngine;
using UnityEngine.UI;

public class GameModeController : MonoBehaviour
{
    public Launcher launcher;
    public Light flashlight;
    public Button launcherModeButton;
    public Button flashlightModeButton;

    private bool isFlashlightMode = false;

    void Start()
    {
        launcherModeButton.onClick.AddListener(SetLauncherMode);
        flashlightModeButton.onClick.AddListener(SetFlashlightMode);

        SetLauncherMode();
    }

    private void SetLauncherMode()
    {
        isFlashlightMode = false;
        launcher.enabled = true;
        flashlight.enabled = false;
        Debug.Log("Switched to Launcher mode.");
    }

    private void SetFlashlightMode()
    {
        isFlashlightMode = true;
        launcher.enabled = false;
        flashlight.enabled = false;
        Debug.Log("Switched to Flashlight mode.");
    }

    void Update()
    {
        if (isFlashlightMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                flashlight.enabled = true;
                Debug.Log("Flashlight turned ON.");
            }

            if (Input.GetMouseButtonUp(0))
            {
                flashlight.enabled = false;
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
}
