
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Updated_ReticleManager2 : MonoBehaviour
{
    [SerializeField] private GameObject reticle;
    [SerializeField] private LayerMask ghostLayer;
    [SerializeField] private FlashlightHitboxManager flashlightManager;
    [SerializeField] private float interactionRadius = 100f;

    private bool isFlashlightEnabled = false;
    private bool isHolyWaterEnabled = false;

    void Update()
    {
        HandleInteraction();
    }

    private void HandleInteraction()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRadius, ghostLayer))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.CompareTag("Ghost"))
            {
                var ghostScript = hitObject.GetComponent<GhostMovement>();
                if (isFlashlightEnabled && !ghostScript.isStunned)
                {
                    ghostScript.StunGhost(4f);
                }
                else if (isHolyWaterEnabled && ghostScript.isStunned)
                {
                    ghostScript.HandleHealth(1);
                }
            }
        }
    }

    public void SelectFlashlight()
    {
        isFlashlightEnabled = true;
        isHolyWaterEnabled = false;
        Debug.Log("Flashlight selected");
        reticle.GetComponent<Image>().color = Color.yellow;
    }

    public void SelectHolyWater()
    {
        isHolyWaterEnabled = true;
        isFlashlightEnabled = false;
        Debug.Log("Holy Water selected");
        reticle.GetComponent<Image>().color = Color.cyan;
    }
}
