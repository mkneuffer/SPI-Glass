using UnityEngine;

public class GhostHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 20; // The ghost's maximum health
    [SerializeField] private int damagePerHit = 2; // Damage caused by each ball
    [SerializeField] private GameObject ballPrefab; // Reference to the ball prefab
    private int currentHealth; // The ghost's current health
    private bool isAlive = true; // Tracks if the ghost is still alive

    void Start()
    {
        // Initialize the ghost's health
        currentHealth = maxHealth;
        Debug.Log($"Ghost spawned with {currentHealth} health.");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object is an instance of the ball prefab
        if (collision.gameObject.CompareTag("Ball") && isAlive)
        {
            TakeDamage(damagePerHit);

            // Optionally destroy the ball upon collision
            Destroy(collision.gameObject);
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Ghost took {damage} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isAlive = false;
        Debug.Log("Ghost is dead.");
        gameObject.SetActive(false); // Hide the ghost
    }
}
