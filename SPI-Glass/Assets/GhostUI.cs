using UnityEngine;
using UnityEngine.UI;

public class GhostUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image progressCircle;
    [SerializeField] private Image healthBar;

    [Header("Ghost References")]
    [SerializeField] private GhostHealth ghost;

    private Camera mainCamera;

    void Start()
    {
        if (ghost != null)
        {
            ghost.OnStunCooldownChanged += HandleStunCooldown; // Listen for cooldown changes
        }

        mainCamera = Camera.main;
    }

    void OnDestroy()
    {
        if (ghost != null)
        {
            ghost.OnStunCooldownChanged -= HandleStunCooldown; // Remove listener to avoid memory leaks
        }
    }

    void Update()
    {
        if (ghost == null) return;

        transform.LookAt(transform.position + mainCamera.transform.forward);

        UpdateFlashlightProgress();
        UpdateHealthBar();
    }

    private void UpdateFlashlightProgress()
    {
        if (progressCircle == null) return;

        if (ghost.IsInCooldown())
        {
            progressCircle.fillAmount = 1f;
            progressCircle.color = Color.black; // Turns black during cooldown
        }
        else
        {
            float progress = ghost.GetFlashlightProgress();
            progressCircle.fillAmount = progress;
            progressCircle.color = Color.Lerp(Color.white, Color.yellow, progress);
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null) return;

        float healthPercent = ghost.GetHealthPercentage();
        healthBar.fillAmount = healthPercent;
        healthBar.color = Color.Lerp(Color.black, Color.red, healthPercent);
    }

    private void HandleStunCooldown(bool isCooldownActive)
    {
        if (isCooldownActive)
        {
            progressCircle.color = Color.black; // Turn black during cooldown
        }
    }
}
