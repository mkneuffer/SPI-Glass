using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightHitboxManager : MonoBehaviour
{

    public GhostMovement ghostMovement;
    public float stunTime = 2.0f; // max stun time
    private bool isStunned = false;
    private float stunTimer = 0f; // logs stun length
    private float time;
    private float lightTime;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    public void StunGhost()
    {
        if (!isStunned) {
            isStunned = true;
            ghostMovement.SetSpeed(0);
            Debug.Log("Ghost is stunned!");
            stunTimer = stunTime;
            StartCoroutine(stunCountdown());
        }
    }


    public void stopStun()
    {
        isStunned = false;
        ghostMovement.SetSpeed(0.01f);
        Debug.Log("Ghost no longer stunned");
    }

    private IEnumerator stunCountdown()
    {
        while (stunTimer > 0)
        {
            stunTimer--;
            yield return new WaitForSeconds(1);
        }
        stopStun();
    }

    public bool getStun()
    {
        return isStunned;
    }
}
