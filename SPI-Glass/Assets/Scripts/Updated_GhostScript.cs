
using System.Collections;
using UnityEngine;

public class Updated_GhostScript : MonoBehaviour
{
    private int health = 5;
    private int round = 1;
    public bool isStunned = false;

    public void HandleHealth(int amount)
    {
        health -= amount;
        Debug.Log($"Health: {health}");

        if (health <= 0)
        {
            if (round < 3)
            {
                round++;
                health = 5;
                Debug.Log($"Round {round} begins!");
            }
            else
            {
                Debug.Log("Ghost defeated!");
                Destroy(gameObject);
            }
        }
    }

    public void StunGhost(float stunTime)
    {
        if (!isStunned)
        {
            isStunned = true;
            Debug.Log("Ghost stunned!");
            StartCoroutine(StunCoroutine(stunTime));
        }
    }

    private IEnumerator StunCoroutine(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
        Debug.Log("Ghost recovered!");
    }
}
