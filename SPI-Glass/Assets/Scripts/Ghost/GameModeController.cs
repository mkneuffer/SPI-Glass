using UnityEngine;
using UnityEngine.UI;

public class GameModeController : MonoBehaviour
{
    public Launcher launcher; // Reference to the launcher script
    public Light flashlight; // Reference to the spotlight in the scene
    public Button launcherModeButton; // UI button for Launcher mode
    public Button flashlightModeButton; // UI button for Flashlight mode

    private bool isFlashlightMode = false; // Tracks if the current mode is Flashlight mode

    void Start()
    {
        // Assign button click events
        launcherModeButton.onClick.AddListener(SetLauncherMode);
        flashlightModeButton.onClick.AddListener(SetFlashlightMode);

        // Start in Launcher mode
        SetLauncherMode();
    }

    private void SetLauncherMode()
    {
        isFlashlightMode = false;
        launcher.enabled = true;
        flashlight.enabled = false; // Ensure flashlight is off in Launcher mode
        Debug.Log("Switched to Launcher mode.");
    }

    private void SetFlashlightMode()
    {
        isFlashlightMode = true;
        launcher.enabled = false;
        flashlight.enabled = false; // Flashlight will only turn on when tapped
        Debug.Log("Switched to Flashlight mode.");
    }

    void Update()
    {
        if (isFlashlightMode)
        {
            // Turn flashlight on when the screen is tapped/held
            if (Input.GetMouseButtonDown(0)) // Screen tapped
            {
                flashlight.enabled = true;
                Debug.Log("Flashlight turned ON.");
            }

            // Turn flashlight off when the screen tap/hold is released
            if (Input.GetMouseButtonUp(0)) // Screen released
            {
                flashlight.enabled = false;
                Debug.Log("Flashlight turned OFF.");
            }

            // Only raycast when the flashlight is on
            if (flashlight.enabled)
            {
                PerformFlashlightRaycast();
            }
        }
    }

    private void PerformFlashlightRaycast()
    {
        // Create a ray from the camera's position forward
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            // Check if the raycast hit a ghost
            GhostHealth ghost = hit.collider.GetComponent<GhostHealth>();
            if (ghost != null)
            {
                ghost.HandleFlashlight(); // Notify the ghost about the flashlight hit
            }
        }
    }
}
