using UnityEngine;

public class GhostPhases : MonoBehaviour
{
    [Header("Phase Settings")]
    [SerializeField] private int[] phaseHealth = { 20, 10, 20 }; // Health for each phase
    [SerializeField] private float[] stunDurations = { 5f, 10f, 5f }; // Stun duration for each phase
    private int currentPhase = 0; // Tracks the current phase (0 = Phase 1, 1 = Phase 2, etc.)

    private int currentHealth; // Tracks the ghost's current health in the current phase
    private bool isStunned = false; // Whether the ghost is stunned
    private bool isAlive = true; // Whether the ghost is alive

    private Animator animator; // Reference to the Animator

    void Start()
    {
        animator = GetComponent<Animator>();
        StartPhase(0); // Begin with Phase 1
    }

    private void StartPhase(int phase)
    {
        if (phase >= phaseHealth.Length)
        {
            Die(); // All phases completed, ghost dies
            return;
        }

        currentPhase = phase;
        currentHealth = phaseHealth[phase];
        Debug.Log($"Starting Phase {phase + 1} with {currentHealth} health and {stunDurations[phase]}s stun duration.");

        // Trigger the phase animation
        animator.SetTrigger($"Phase{phase + 1}");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball") && isStunned && isAlive)
        {
            TakeDamage(2); // Adjust ball damage as needed
            Destroy(collision.gameObject); // Destroy the ball on collision
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
        StartPhase(currentPhase + 1); // Advance to the next phase
    }

    public void Stun()
    {
        if (isStunned) return;

        Debug.Log($"Ghost is stunned for {stunDurations[currentPhase]} seconds.");
        animator.SetTrigger("Stun"); // Trigger stun animation
        isStunned = true;

        // Start stun timer
        Invoke(nameof(EndStun), stunDurations[currentPhase]);
    }

    private void EndStun()
    {
        isStunned = false;
        Debug.Log("Ghost is no longer stunned.");
    }

    private void Die()
    {
        isAlive = false;
        animator.SetTrigger("Die"); // Trigger death animation
        Debug.Log("Ghost is dead.");
    }
}
