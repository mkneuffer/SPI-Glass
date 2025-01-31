using UnityEngine;

public class GhostHealth : MonoBehaviour
{
    [Header("Phase Settings")]
    [SerializeField] private int[] phaseHealth = { 20, 10, 20 };
    [SerializeField] private float[] stunDurations = { 5f, 10f, 5f };

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
    private float flashlightTimer = 0f;
    private float flashlightThreshold = 5f;

    private string lastMovementAnimation; // Stores last movement animation before stun
    private float lastAnimationTime; // Stores animation time before pausing

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
        Debug.Log($"Starting Phase {phase + 1} with {currentHealth} HP, Stun {stunDurations[phase]}s.");

        ghostAnimator.Play("Float"); // Looping float animation
        PlayRandomMovementAnimation();
    }

    private void PlayRandomMovementAnimation()
    {
        if (currentPhase < phaseAnimations.Length)
        {
            string[] animations = phaseAnimations[currentPhase];
            if (animations.Length > 0)
            {
                lastMovementAnimation = animations[Random.Range(0, animations.Length)];
                movementAnimator.CrossFade(lastMovementAnimation, 0.2f);
                Debug.Log($"Playing movement animation: {lastMovementAnimation}");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision detected with: {collision.gameObject.name} (Tag: {collision.gameObject.tag})");

        if (collision.gameObject.CompareTag("Ball"))
        {
            Debug.Log("Ball hit detected.");

            if (isStunned && isAlive)
            {
                Debug.Log("Ghost is stunned. Taking 2 damage.");
                TakeDamage(2);
                Destroy(collision.gameObject, 0.1f);
            }
            else
            {
                Debug.Log("Ball hit ghost, but no damage (not stunned).");
            }
        }
    }

    public void HandleFlashlight()
    {
        if (isStunned || !isAlive)
        {
            Debug.Log("Flashlight hit ghost but it is stunned or dead. No effect.");
            return;
        }

        flashlightTimer += Time.deltaTime;
        Debug.Log($"Flashlight on ghost: {flashlightTimer:F2} seconds");

        if (flashlightTimer >= flashlightThreshold)
        {
            StunGhost();
        }
    }

    private void StunGhost()
    {
        isStunned = true;
        flashlightTimer = 0f;
        Debug.Log($"Ghost is stunned for {stunDurations[currentPhase]} seconds.");

        ghostAnimator.Play("Stun"); // Looping Stun animation

        // Pause movement animation by storing current time and stopping playback
        AnimatorStateInfo stateInfo = movementAnimator.GetCurrentAnimatorStateInfo(0);
        lastMovementAnimation = stateInfo.IsName(lastMovementAnimation) ? lastMovementAnimation : stateInfo.fullPathHash.ToString();
        lastAnimationTime = stateInfo.normalizedTime;
        
        movementAnimator.speed = 0; // Pause movement animations

        Invoke(nameof(EndStun), stunDurations[currentPhase]);
    }

    private void EndStun()
    {
        isStunned = false;
        Debug.Log("Ghost is no longer stunned.");

        movementAnimator.speed = 1; // Resume movement animations
        ghostAnimator.Play("Float"); // Resume floating animation

        // Resume movement animation from where it was paused
        if (!string.IsNullOrEmpty(lastMovementAnimation))
        {
            movementAnimator.Play(lastMovementAnimation, 0, lastAnimationTime);
        }
        else
        {
            PlayRandomMovementAnimation(); // Pick a new one if needed
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Ghost took {damage} damage. Current health: {currentHealth}");

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
}
