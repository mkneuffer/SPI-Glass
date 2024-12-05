using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightHitboxManager : MonoBehaviour
{

    public GhostMovement ghostMovement;
    public float stunTime = 10.0f; // max stun time
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

    public void DoFlashlightDamage()
    {
        if (!isStunned)
        {
            ghostMovement.TakeFlashlightDamage(1);
        }
        if (ghostMovement.GetFlashlightHealth() <= 0 && !isStunned)
        {
            isStunned = true;
            ghostMovement.isStunned = true;
            ghostMovement.SetSpeed(0);
            Debug.Log($"Ghost stun is {isStunned}");
            stunTimer = stunTime;
            ghostMovement.ResetFlashlightHealth();
            StartCoroutine(stunCountdown());
        }
    }


    public void stopStun()
    {
        isStunned = false;
        ghostMovement.isStunned = false;
        ghostMovement.ResetSpeed();
        Debug.Log($"Ghost stun is {isStunned}");
    }

    private IEnumerator stunCountdown()
    {
        while (stunTimer > 0)
        {
            stunTimer--;
            yield return new WaitForSeconds(1);
        }
        if (ghostMovement.isStunned)
        {
            stopStun();
        }
    }

    public bool getStun()
    {
        return isStunned;
    }
}
