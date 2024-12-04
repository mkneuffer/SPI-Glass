using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HolyWaterHitboxManager : MonoBehaviour
{
    public GhostMovement ghostMovement;
    public int damage = 1;
    private bool alreadyHit = false;

    public void ApplyDamage() {
        if (ghostMovement != null) {
            ghostMovement.HandleHealth(damage);
            Debug.Log("Damage dealt");
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if (!alreadyHit && other.CompareTag("Ghost")) 
        {
            GhostMovement ghostMovement = other.GetComponent<GhostMovement>();
            if (ghostMovement != null && ghostMovement.isStunned) 
            {
                alreadyHit = true;
                StartCoroutine(DestroyAfterFrame());
                ApplyDamage();
                Debug.Log("Damaged Applied!");
            }
        }
    }

    private IEnumerator DestroyAfterFrame()
    {
        yield return null;
        Destroy(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
