using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.Controls;
using TreeEditor;

public class GhostMovement : MonoBehaviour
{

    float WRadius = .005f; //How close the ghost has to be to a point to count as being at that point
    int counter = 0;
    float BezierCurveT = 0; //Way through each bezier curve
    //private Vector3[] diamond = { Vector3.up, Vector3.right, Vector3.down, Vector3.left };
    public float currentSpeed; //Value should be around .01
    [SerializeField] float defaultSpeed;
    private Vector3 startingPosition;
    private Vector3 previousWaypoint;
    [SerializeField] private WaypointStorage[] paths;
    [SerializeField] FlashlightHitboxManager flashlight;
    [SerializeField] private Animator transition;

    //[SerializeField] private TextMeshProUGUI ghostHealthTextUI;

    [SerializeField] private int health = 10;
    private int flashlightHealth = 60;
    private int maxFlashlightHealth;
    private int phase = 1;
    public bool isStunned = false;
    private bool isVulnerable = false;
    private bool transitioningPhase = false;

    // Start is called before the first frame update
    void Start()
    {
        //transform.Rotate(0.0f, 180.0f, 0.0f, Space.Self);
        startingPosition = transform.position;
        previousWaypoint = startingPosition;
        BezierCurveT = 0;
        maxFlashlightHealth = flashlightHealth;
        currentSpeed = defaultSpeed;

        //Invoke("SwapPath", 7);
    }
    // Update is called once per frame
    void Update()
    {

        if (!isStunned && !transitioningPhase && phase <= 3)
        {
            MoveToPoints(paths[phase - 1].GetWaypoints());
        }
    }

    //Moves the ghost along the given waypoints
    //The waypoints are relative to the starting position of the ghost
    void MoveToPoints(Waypoints[] waypoints)
    {

        if (Vector3.Distance(startingPosition + waypoints[counter].point, transform.position) < WRadius || BezierCurveT > 1)
        {
            previousWaypoint = transform.position;
            BezierCurveT = currentSpeed;
            counter++;
            if (counter >= waypoints.Length)
            {
                counter = 0;
            }
        }

        Vector3 P0 = previousWaypoint;
        Vector3 P3 = startingPosition + waypoints[counter].point;

        Vector3 P1 = new Vector3();
        Vector3 P2 = new Vector3();
        Vector3 midpoint = (P0 + P3) / 2;
        if (waypoints[counter].curvesUp)
        {

            P1 = new Vector3(midpoint.x, P0.y, midpoint.z);
            P2 = new Vector3(P3.x, midpoint.y, P3.z);
        }
        else
        {
            P1 = new Vector3(P0.x, midpoint.y, P0.z);
            P2 = new Vector3(midpoint.x, P3.y, midpoint.z);
        }
        transform.position = BezierCurve(BezierCurveT, P0, P1, P2, P3);
        BezierCurveT += currentSpeed;

    }



    Vector3 BezierCurve(float t, Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3)
    {
        Vector3 output = Mathf.Pow((1 - t), 3) * P0 + 3 * Mathf.Pow((1 - t), 2) * t * P1 + 3 * (1 - t) * t * t * P2 + Mathf.Pow(t, 3) * P3;
        return output;
    }

    public void SetSpeed(float change)
    {
        currentSpeed = change;
    }

    float GetSpeed()
    {
        return currentSpeed;
    }

    public void ResetSpeed()
    {
        if (phase == 3)
        {
            currentSpeed = phase * 2 * defaultSpeed;
        }
        else if (phase == 2)
        {
            currentSpeed = phase * defaultSpeed;
        }
    }

    void multSpeed(float mult)
    {
        currentSpeed *= mult;
    }

    //Deals with the health value of the ghost
    //Does damage if touched/tapped
    //Ghost is destroyed if health <= 0
    public void HandleHealth(int amount)
    {
        if (!isStunned)
        {
            Debug.Log("Not stunned!");
            return;
        }

        health -= amount;
        Debug.Log("Health: " + health);
        if (health <= 0)
        {
            isStunned = false;
            health = 10;
            phase++;
            GoToNextPhase();
            flashlight.stopStun();
            Debug.Log("Phase:" + phase);
            if (phase > 3)
            {
                Debug.Log("End fight");
                StartCoroutine(LoadScene5());
            }
            /*    else
                  {
                      health = 10;
                      //Debug.Log("Phase: " + phase);
                      SwapPath();
                      flashlight.stopStun();
                  } */
        }


    }

    IEnumerator LoadScene5()
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(3f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(7);
    }

    //Swaps between two different waypoints
    void GoToNextPhase()
    {
        if (phase > 3)
        {
            return;
        }

        counter = 0;
        BezierCurveT = 0;
        previousWaypoint = startingPosition;
        transitioningPhase = true;
        StartCoroutine(MoveToPosition(startingPosition, .07f));

    }

    private IEnumerator MoveToPosition(Vector3 moveTo, float speed)
    {
        Debug.Log("corutine");
        while (Vector3.Distance(moveTo, transform.position) > WRadius)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveTo, speed);
            yield return new WaitForSeconds(.01f);
        }
        yield return new WaitForSeconds(.5f);
        transitioningPhase = false;
    }

    public void StunGhost(float stunTime)
    {
        if (!isStunned)
        {
            isStunned = true;
            StartCoroutine(StunCoroutine(stunTime));
        }
    }

    private IEnumerator StunCoroutine(float stunTime)
    {
        Debug.Log($"Ghost stunned for {stunTime} seconds.");
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
        Debug.Log("Ghost recovered!");
    }

    private IEnumerator HealthCoroutine(float playerHealth)
    {
        playerHealth--;
        //Debug.Log($"Current health: {health}");
        yield return new WaitForSeconds(1);
    }

    public void TakeFlashlightDamage(int damage)
    {
        flashlightHealth -= damage;
    }

    public int GetFlashlightHealth()
    {
        return flashlightHealth;
    }

    public void ResetFlashlightHealth()
    {
        flashlightHealth = maxFlashlightHealth;
    }

    public void SkipScene()
    {
        StartCoroutine(LoadScene5());
    }
}


