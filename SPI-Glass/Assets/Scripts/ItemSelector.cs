using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelector : MonoBehaviour
{
    [SerializeField] private FlashlightHitboxManager flashlightManager;
    [SerializeField] private HolyWaterHitboxManager holyWaterManager;
    private bool isFlashlightActive = true;

    public void OnInteract() {
        if (isFlashlightActive)
        {
            flashlightManager.StunGhost();
        }
        else
        {
            holyWaterManager.ApplyDamage();
        }
    }

    public void SelectFlashlight() {
        isFlashlightActive = true;
        flashlightManager.enabled = true;
        holyWaterManager.enabled = false;
        Debug.Log("Flashlight is active");
    }

        public void SelectHolyWater() {
        isFlashlightActive = false;
        flashlightManager.enabled = false;
        holyWaterManager.enabled = true;
        Debug.Log("Holy Water is active");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
