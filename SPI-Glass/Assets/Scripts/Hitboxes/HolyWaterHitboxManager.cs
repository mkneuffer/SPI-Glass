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

    void OnTriggerEnter(Collider other) {
        if (!alreadyHit && other.CompareTag("Ghost")) 
        {
            GhostMovement ghostMovement = other.GetComponent<GhostMovement>();
            if (ghostMovement != null && ghostMovement.isStunned) 
            {
                ApplyDamage();
                Debug.Log("Damaged Applied!");
                Destroy(gameObject);
                StartCoroutine(ResetDamageFlag());
            }
        }
    }

    private IEnumerator ResetDamageFlag()
    {
        yield return new WaitForEndOfFrame();
        alreadyHit = false;
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
