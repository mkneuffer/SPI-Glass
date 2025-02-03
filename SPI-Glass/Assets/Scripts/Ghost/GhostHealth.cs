using UnityEngine;

public class GhostHealth : MonoBehaviour
{
    [Header("Phase Settings")]
    [SerializeField] private int[] phaseHealth = { 20, 10, 20 };
    [SerializeField] private float[] stunDurations = { 5f, 10f, 5f };
    [SerializeField] private float[] flashlightThresholds = { 3f, 5f, 7f };

    [Header("Cooldown Settings")]
    [SerializeField] private float stunCooldownDuration = 3f; // 3 seconds of immunity after stun

    [Header("References")]
    [SerializeField] private Animator ghostAnimator;
    [SerializeField] private Animator movementAnimator;
    [SerializeField] private Rigidbody ghostRigidbody;
    [SerializeField] private Collider selectedCollider;

    [Header("Movement Animations")]
    [SerializeField] private string[][] phaseAnimations =
    {
        new string[] { "Phase1A", "Phase1B", "Phase1C" },
        new string[] { "Phase2A", "Phase2B", "Phase2C" },
        new string[] { "Phase3A", "Phase3B", "Phase3C" }
    };

    private int currentPhase = 0;
    private int currentHealth;
    private bool isAlive = true;
    private bool isStunned = false;
    private bool isInCooldown = false; // Cooldown status
    private float flashlightTimer = 0f;

    private string lastMovementAnimation = "";

    // UI Events
    public delegate void StunCooldownEvent(bool isCooldownActive);
    public event StunCooldownEvent OnStunCooldownChanged;

    public delegate void GhostSpawnEvent();
    public event GhostSpawnEvent OnGhostSpawned;

    void Start()
    {
        if (selectedCollider == null)
        {
            Debug.LogError("No Collider assigned in Inspector! Please assign one.");
        }

        ghostRigidbody = GetComponent<Rigidbody>();
        if (ghostRigidbody == null)
        {
            Debug.LogWarning("Ghost is missing a Rigidbody. Adding a kinematic one.");
            ghostRigidbody = gameObject.AddComponent<Rigidbody>();
            ghostRigidbody.isKinematic = true;
        }

        OnGhostSpawned?.Invoke(); // Notify UI that ghost has spawned
        StartPhase(0);
    }

    private void StartPhase(int phase)
    {
        if (phase >= phaseHealth.Length)
        {
            Die();
            return;
        }

        currentPhase = phase;
        currentHealth = phaseHealth[phase];
        isStunned = false;
        isInCooldown = false; // Reset cooldown on phase change
        flashlightTimer = 0f;

        Debug.Log($"Starting Phase {phase + 1} | HP: {currentHealth} | Stun Time: {stunDurations[phase]}s | Flashlight Threshold: {flashlightThresholds[phase]}s");

        ghostAnimator.Play("Float");
        PlayNextMovementAnimation();
    }

    private void PlayNextMovementAnimation()
    {
        if (currentPhase < phaseAnimations.Length)
        {
            string[] animations = phaseAnimations[currentPhase];

            if (animations.Length > 0)
            {
                string nextAnimation;
                do
                {
                    nextAnimation = animations[Random.Range(0, animations.Length)];
                } while (nextAnimation == lastMovementAnimation);

                lastMovementAnimation = nextAnimation;
                movementAnimator.CrossFade(nextAnimation, 0.2f);
                Debug.Log($"Starting movement animation: {nextAnimation}");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision detected with: {collision.gameObject.name} (Tag: {collision.gameObject.tag})");

        if (collision.gameObject.CompareTag("Ball"))
        {
            if (isStunned && isAlive)
            {
                Debug.Log("Ghost is stunned. Taking 2 damage.");
                TakeDamage(2);
                Destroy(collision.gameObject, 0.1f);
            }
        }
    }

    public void HandleFlashlight()
    {
        if (isStunned || !isAlive || isInCooldown)
        {
            Debug.Log("Flashlight hit ghost but it is stunned, dead, or in cooldown. No effect.");
            return;
        }

        flashlightTimer += Time.deltaTime;
        Debug.Log($"Flashlight on ghost: {flashlightTimer:F2}s");

        if (flashlightTimer >= flashlightThresholds[currentPhase])
        {
            StunGhost();
        }
    }

    private void StunGhost()
    {
        isStunned = true;
        flashlightTimer = 0f;

        Debug.Log($"Ghost is stunned for {stunDurations[currentPhase]} seconds.");
        ghostAnimator.Play("Stun");
        movementAnimator.speed = 0;

        Invoke(nameof(EndStun), stunDurations[currentPhase]);
    }

    private void EndStun()
    {
        isStunned = false;
        isInCooldown = true; // Enter cooldown phase
        Debug.Log("Ghost is no longer stunned. Entering cooldown.");

        movementAnimator.speed = 1;
        ghostAnimator.Play("Float");

        OnStunCooldownChanged?.Invoke(true); // Notify UI that cooldown started

        Invoke(nameof(EndCooldown), stunCooldownDuration);
    }

    private void EndCooldown()
    {
        isInCooldown = false;
        Debug.Log("Cooldown ended. Ghost can now be stunned again.");

        OnStunCooldownChanged?.Invoke(false); // Notify UI that cooldown ended
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Ghost took {damage} damage. Remaining HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            AdvancePhase();
        }
    }

    private void AdvancePhase()
    {
        Debug.Log($"Phase {currentPhase + 1} completed.");
        StartPhase(currentPhase + 1);
    }

    private void Die()
    {
        isAlive = false;
        Debug.Log("Ghost is dead.");
        gameObject.SetActive(false);
    }

    public bool IsStunned()
    {
        return isStunned;
    }

    public bool IsInCooldown()
    {
        return isInCooldown;
    }

    public float GetFlashlightProgress()
    {
        return isInCooldown ? 0 : Mathf.Clamp01(flashlightTimer / flashlightThresholds[currentPhase]); 
    }

    public float GetHealthPercentage()
    {
        return Mathf.Clamp01((float)currentHealth / phaseHealth[currentPhase]);
    }

    void Update()
    {
        if (!isStunned && !isInCooldown)
        {
            AnimatorStateInfo stateInfo = movementAnimator.GetCurrentAnimatorStateInfo(0);

            // Ensure animation fully completes before switching
            if (stateInfo.normalizedTime >= 1.0f && stateInfo.IsTag("Move"))
            {
                PlayNextMovementAnimation();
            }
        }
    }
}
