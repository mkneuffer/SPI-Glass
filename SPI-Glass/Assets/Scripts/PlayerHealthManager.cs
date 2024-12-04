using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{

    [SerializeField] private int playerHealth = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator HealthCoroutine(int playerHealth)
    {
        playerHealth--;
        Debug.Log($"Current health: {playerHealth}");
        yield return new WaitForSeconds(1f);
    }

    public void healthManager(int playerHealth)
    {
        while(playerHealth > 0)
        {
            StartCoroutine(HealthCoroutine(playerHealth));
        }
        if(playerHealth == 0)
        {
            Debug.Log("No health left!");
        }
    }
}
