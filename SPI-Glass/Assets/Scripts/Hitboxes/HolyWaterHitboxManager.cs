using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyWaterHitboxManager : MonoBehaviour
{
    public GhostMovement ghostMovement;
    public int damage = 1;

    public void ApplyDamage() {
        ghostMovement.HandleHealth(damage);
        Debug.Log("Damage dealt");
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
