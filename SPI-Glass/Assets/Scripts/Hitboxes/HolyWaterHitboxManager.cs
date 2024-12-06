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
    [SerializeField] private float lifetime = 10f;

    public void ApplyDamage() 
    {
        if (ghostMovement != null) 
        {
            alreadyHit = true;
            ghostMovement.HandleHealth(damage);
            Debug.Log("Damage dealt");
            Destroy(splashEffect);
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
                alreadyHit = true;
                this.ghostMovement = ghostMovement;
                ApplyDamage();
                PlaySplashEffect();
                Destroy(gameObject);
            }
        }
    }

    private void PlaySplashEffect()
    {
        if(splashEffect != null)
        {
            GameObject splash = Instantiate(splashEffect, transform.position, Quaternion.identity);
            Destroy(splash, splash.GetComponent<ParticleSystem>()?.main.duration ?? 1f);
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
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
