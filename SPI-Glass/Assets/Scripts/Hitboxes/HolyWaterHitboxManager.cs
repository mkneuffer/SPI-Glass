using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HolyWaterHitboxManager : MonoBehaviour
{
    public GhostMovement ghostMovement;
    public Animator throwAnimator;
    public int damage = 1;
    private bool alreadyHit = false;
    [SerializeField] private GameObject splashEffect;

    public void ApplyDamage() 
    {
        if (ghostMovement != null) 
        {
            alreadyHit = true;
            ghostMovement.HandleHealth(damage);
            Debug.Log("Damage dealt");
            Destroy(gameObject);

            if (splashEffect != null && ghostMovement != null)
            {
                Debug.Log("SPLASH!");
                GameObject splash = Instantiate(splashEffect, ghostMovement.transform.position, Quaternion.identity);
                Destroy(splash, 0.5f);
            }

            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if(alreadyHit)
        {
            return;
        }

        if (other.CompareTag("Ghost")) 
        {
            GhostMovement ghostMovement = other.GetComponent<GhostMovement>();
            if (ghostMovement != null && ghostMovement.isStunned) 
            {
                this.ghostMovement = ghostMovement;
                ApplyDamage();
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
        if (throwAnimator == null)
        {
            throwAnimator = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
