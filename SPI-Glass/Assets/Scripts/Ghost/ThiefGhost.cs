using System;
using System.Collections;
using System.Linq.Expressions;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class ThiefGhost : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Animator ghostAnimator;
    [SerializeField] private Animator movementAnimator;
    [SerializeField] private Rigidbody ghostRigidbody;
    [SerializeField] private Collider selectedCollider;
    [SerializeField] private GameObject ropeOnGhost;

    [Header("Fog Animation & Scene Transition")]
    [SerializeField] private GameObject fogObject; // Assign fog object in the Inspector
    [SerializeField] private string nextSceneName; // Assign next scene in the Inspector


    private int currentPhase = 0;
    private int totalPhases = 3;
    private bool isAlive = true;
    private bool isStunned = false;
    private bool isInCooldown = false; // Cooldown status
    private bool isRoped = false;
    private bool trapped = false;
    private float stunCooldown = 0.1f;


    // UI Events
    public delegate void StunCooldownEvent(bool isCooldownActive);
    public event StunCooldownEvent OnStunCooldownChanged;

    public delegate void GhostSpawnEvent();
    public event GhostSpawnEvent OnGhostSpawned;
    private GameObject ghostAnchor;
    private Vector3 direction;

    [SerializeField] private float defaultSpeed = 5;
    [SerializeField] private float speed;
    [SerializeField] private GameObject ghostModel;



    void Start()
    {
        speed = defaultSpeed;
        ghostAnchor = transform.GetChild(0).gameObject;
        movementAnimator.speed = 0;
        if (selectedCollider == null)
        {
            Debug.LogError("No Collider assigned in Inspector! Please assign one.");
        }

        ghostRigidbody = GetComponent<Rigidbody>();
        if (ghostRigidbody == null)
        {
            Debug.LogWarning("Ghost is missing a Rigidbody. Adding a kinematic one.");
            ghostRigidbody = gameObject.AddComponent<Rigidbody>();
            ghostRigidbody.isKinematic = true;
        }

        StartPhase(0);
    }

    private void StartPhase(int phase)
    {
        if (phase >= totalPhases)
        {
            Die();
            return;
        }

        float angle = Random.Range(0, 2 * Mathf.PI);
        direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        currentPhase = phase;
        isStunned = false;
        isInCooldown = false; // Reset cooldown on phase change
        isRoped = false;
        trapped = false;

        if (phase != 0)
        {
            var laserAndMirrorManager = GameObject.Find("XR Origin").GetComponent<ARLaserAndMirrorManager>();
            laserAndMirrorManager.DeleteAllObjects();
        }


        speed = defaultSpeed;
        Debug.Log($"Starting Phase {phase + 1}");
        ghostAnimator.speed = 1;
        ghostAnimator.Play("CrouchWalk");
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision detected with: {collision.gameObject.name} (Tag: {collision.gameObject.tag})");

        if (collision.gameObject.CompareTag("Rope") && !isRoped && trapped)
        {
            if (isAlive)
            {
                isRoped = true;
                DestoryTrueParent(collision.gameObject);

                GameObject rope = Instantiate(ropeOnGhost, transform.GetChild(0));
                rope.transform.position += new Vector3(-0.05f, 1f);
                rope.tag = "DestroyThis";
                Invoke(nameof(AdvancePhase), 1f);

            }
        }
    }

    //Given an object, destorys it's entire heirarchy
    private void DestoryTrueParent(GameObject gameObject)
    {
        while (gameObject.transform.parent != null)
        {
            gameObject = gameObject.transform.parent.gameObject;
        }
        Destroy(gameObject, 0.2f);
    }

    //Stuns the ghost
    public void StunGhost()
    {
        if (isStunned || isInCooldown)
        {
            return;
        }
        isStunned = true;
        direction = direction * -1;
        Debug.Log($"Ghost is stunned.");
        ghostAnimator.Play("CrouchIdle");
        speed = 0;
        //After 3 seconds, end the stun
        Invoke(nameof(EndStun), 3f);
    }

    //Stops the ghost from being stunned and starts a cooldown for when it can be stunned again
    private void EndStun()
    {
        if (!trapped)
        {
            isStunned = false;
            isInCooldown = true; // Enter cooldown phase
            Debug.Log("Ghost is no longer stunned. Entering cooldown.");

            ghostAnimator.Play("CrouchWalk");

            speed = defaultSpeed;
            Invoke(nameof(EndCooldown), stunCooldown);
        }
    }


    //Ends the cooldown between stuns and allows the ghost to be stunned again
    private void EndCooldown()
    {
        isInCooldown = false;
        Debug.Log("Cooldown ended. Ghost can now be stunned again.");
    }

    //Updates whether the ghost is trapped or not
    //If new value is the same as old value, just do nothing
    public void UpdateTrapped(bool isTrapped)
    {
        if (trapped == isTrapped)
        {
            return;
        }
        trapped = isTrapped;
        if (isTrapped)
        {
            ghostAnimator.Play("CrouchIdle");
            speed = 0;
        }
        else if (!isStunned)
        {
            ghostAnimator.Play("CrouchWalk");
            speed = defaultSpeed;
        }
        Debug.Log($"Trapped is set to {trapped}");
    }

    //Goes to the next phase of the fight
    private void AdvancePhase()
    {
        Debug.Log($"Phase {currentPhase + 1} completed.");
        StartPhase(currentPhase + 1);
    }

    //Ghost dies
    //Go to next scene after 3 seconds
    private void Die()
    {
        if (!isAlive) return; // Prevent multiple calls
        isAlive = false;
        Debug.Log("Ghost is dead. Playing fog animation and switching scene...");

        // Play the fog animation
        if (fogObject != null)
        {
            Animator fogAnimator = fogObject.GetComponent<Animator>();
            if (fogAnimator != null)
            {
                fogAnimator.Play("FogIn"); // Play the fog animation
            }
            else
            {
                Debug.LogWarning("Fog object does not have an Animator component!");
            }
        }
        else
        {
            Debug.LogWarning("Fog object not assigned!");
        }

        // Schedule scene change after 3 seconds
        Invoke(nameof(SwitchScene), 3f);
    }

    //Switches scene to scene corresponding to nextSceneName
    private void SwitchScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log($"Switching to scene: {nextSceneName}");
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Next scene name not assigned in the Inspector!");
        }
    }

    public bool IsStunned()
    {
        return isStunned;
    }

    public bool IsInCooldown()
    {
        return isInCooldown;
    }

    void Update()
    {
        ghostModel.transform.rotation = Quaternion.LookRotation(direction);
        ghostModel.transform.Rotate(new Vector3(0, -90));
        GhostMovement();
    }

    //Handles the thief ghost's movement
    //Ghost moves around on the floor and bounces off of walls
    private void GhostMovement()
    {

        if (ghostAnchor.transform.localPosition.x > 5)
        {
            if (canXReflect)
            {
                StartCoroutine(XReflect());
                direction = Vector3.Reflect(direction, new Vector3(-1, 0, 0));
            }
        }
        else if (ghostAnchor.transform.localPosition.x < -5)
        {
            if (canXReflect)
            {
                StartCoroutine(XReflect());
                direction = Vector3.Reflect(direction, new Vector3(1, 0, 0));
            }
        }
        if (ghostAnchor.transform.localPosition.z > 5)
        {
            if (canZReflect)
            {
                StartCoroutine(ZReflect());
                direction = Vector3.Reflect(direction, new Vector3(0, 0, 1));
            }
        }
        else if (ghostAnchor.transform.localPosition.z < -5)
        {
            if (canZReflect)
            {
                StartCoroutine(ZReflect());
                direction = Vector3.Reflect(direction, new Vector3(0, 0, -1));
            }
        }
        ghostAnchor.transform.localPosition += Vector3.Normalize(direction) * speed * Time.deltaTime;
    }

    private bool canXReflect = true;
    private bool canZReflect = true;
    IEnumerator XReflect()
    {
        canXReflect = false;
        yield return new WaitForSeconds(1);
        canXReflect = true;
    }

    IEnumerator ZReflect()
    {
        canZReflect = false;
        yield return new WaitForSeconds(1);
        canZReflect = true;
    }
}
