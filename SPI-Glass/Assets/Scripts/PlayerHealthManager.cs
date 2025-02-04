using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerHealthManager : MonoBehaviour
{

    [SerializeField] private int maxHealth;
    [SerializeField] private int playerHealth;
    [SerializeField] private float drainRate = 1.0f;
    [SerializeField] private Volume postProcessingVolume;
    [SerializeField] private Animator transition;

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
        StartCoroutine(RestartScene());
        // add game over scene?
    }

    IEnumerator RestartScene()
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(3f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Stump 1");
    }

    private IEnumerator HealthCoroutine()
    {
        while(isDrain && playerHealth > 0) 
        {
            yield return new WaitForSeconds(drainRate);
            playerHealth--;
            UpdateVignette();
            if(playerHealth <= 0) 
            {
                Debug.Log("Health ran out!");
                StopHealthDrain();
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
