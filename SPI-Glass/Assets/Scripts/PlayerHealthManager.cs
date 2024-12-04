using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerHealthManager : MonoBehaviour
{

    [SerializeField] private int maxHealth = 120;
    [SerializeField] private int playerHealth;
    [SerializeField] private float drainRate = 1.0f;
    [SerializeField] private Volume postProcessingVolume;

    private Vignette vignette;
    private bool isDrain = false;

    // Start is called before the first frame update
    void Start()
    {
        if (postProcessingVolume.profile.TryGet<Vignette>(out var vignetteEffect))
        {
            vignette = vignetteEffect;
        }
        else
        {
            Debug.LogError("Vignette effect not found in Post-Processing Volume!");
        }

        playerHealth = maxHealth;
        StartHealthDrain();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartHealthDrain() 
    {
        if(!isDrain) 
        {
            isDrain = true;
            StartCoroutine(HealthCoroutine());
        }
    }

    public void StopHealthDrain() 
    {
        isDrain = false;
        // whatever function to end game/restart scene
    }

    private IEnumerator HealthCoroutine()
    {
        while(isDrain && playerHealth > 0) 
        {
            yield return new WaitForSeconds(drainRate);
            playerHealth--;
            //Debug.Log($"Current health: {playerHealth}");
            UpdateVignette();
            if(playerHealth <= 0) 
            {
                Debug.Log("Health ran out!");
                StopHealthDrain(); // Can probably just loop back to earlier scene?
            }
        }
    }

    // Might need this later...
    public void ModifyHealth(int amount)
    {
        playerHealth += amount;
        Debug.Log($"Player health adjusted by {amount}. New health: {playerHealth}");

        if (playerHealth <= 0)
        {
            Debug.Log("Player has no health left!");
            StopHealthDrain();
        }
    }

    private void UpdateVignette()
    {
        if(vignette != null)
        {
            float healthPercentage = (float)playerHealth / maxHealth;
            vignette.intensity.value = Mathf.Lerp(0.6f, 0f, healthPercentage);
        }
    }
}
