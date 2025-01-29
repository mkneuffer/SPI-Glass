using UnityEngine;

public class GhostHealth : MonoBehaviour
{
    [Header("Phase Settings")]
    [SerializeField] private int[] phaseHealth = { 20, 10, 20 };
    [SerializeField] private float[] stunDurations = { 5f, 10f, 5f };

    [Header("Animation References")]
    [SerializeField] private Animator ghostAnimator;
    [SerializeField] private Animator movementAnimator;

    private Collider ghostCollider;
    private int currentPhase = 0;
    private int currentHealth;
    private bool isAlive = true;
    private bool isStunned = false;
    private float flashlightTimer = 0f;
    private float flashlightThreshold = 5f;

    void Start()
    {
        // Find the first active collider in the children
        ghostCollider = GetComponentInChildren<Collider>();

        if (ghostCollider == null)
        {
            Debug.LogError("No Collider Found! Make sure the ghost has a collider as a child.");
        }
        else
        {
            Debug.Log($"Ghost Collider Found: {ghostCollider.gameObject.name}");
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

        switch (phase)
        {
            case 0:
                movementAnimator.SetTrigger("Phase1");
                break;
            case 1:
                movementAnimator.SetTrigger("Phase2");
                break;
            case 2:
                movementAnimator.SetTrigger("Phase3");
                break;
        }

        ghostAnimator.SetTrigger("Float");
    }

    private void OnCollisionEnter(Collision collision)
{
    Debug.Log($"🔴 Collision Detected with: {collision.gameObject.name} (Tag: {collision.gameObject.tag})");

    if (collision.gameObject.CompareTag("Ball"))
    {
        Debug.Log("⚽ Ball hit detected!");

        if (isStunned && isAlive) // ✅ Ball does damage when ghost is stunned
        {
            Debug.Log("💀 Ghost is stunned! Taking damage from ball.");
            TakeDamage(2);
            Destroy(collision.gameObject);
        }
        else
        {
            Debug.Log("🚫 Ghost is NOT stunned. Ball does no damage.");
        }
    }
}

private void OnTriggerEnter(Collider other)
{
    Debug.Log($"🔴 Trigger Enter Detected with: {other.gameObject.name} (Tag: {other.gameObject.tag})");

    if (other.CompareTag("Ball"))
    {
        Debug.Log("⚽ Ball entered trigger zone!");

        if (isStunned && isAlive)
        {
            Debug.Log("💀 Ghost is stunned! Taking damage from ball.");
            TakeDamage(2);
            Destroy(other.gameObject);
        }
        else
        {
            Debug.Log("🚫 Ghost is NOT stunned. Ball does no damage.");
        }
    }
}



    public void HandleFlashlight()
    {
        if (isStunned || !isAlive) 
        {
            Debug.Log("Flashlight hit ghost but it is stunned or dead - No effect.");
            return; // Flashlight cannot stun a ghost that is already stunned or dead
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

        ghostAnimator.SetTrigger("Stun");
        movementAnimator.speed = 0;

        Invoke(nameof(EndStun), stunDurations[currentPhase]);
    }

    private void EndStun()
    {
        isStunned = false;
        Debug.Log("Ghost is no longer stunned.");

        movementAnimator.speed = 1;
        ghostAnimator.SetTrigger("Float");
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
