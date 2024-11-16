using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightHitboxManager : MonoBehaviour
{

    public GhostMovement ghostMovement;
    public float stunTime = 12.0f; // max stun time
    private bool isStunned = false;
    private float stunTimer = 0f; // logs stun length


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isStunned) {
            stunTimer += Time.deltaTime;
            if(stunTimer >= stunTime) {
                isStunned = false;
                ghostMovement.enabled = true;
                stunTimer = 0f;
            }
        }
    }

    public void StunGhost()
    {
        if (!isStunned) {
            isStunned = true;
            ghostMovement.enabled = false;
            Debug.Log("Ghost is stunned!");
            StartCoroutine(ResumeAfterStun());
        }
    }

    private IEnumerator ResumeAfterStun() {
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
        ghostMovement.enabled = true;
        Debug.Log("Ghost recovered!");
    }
}
