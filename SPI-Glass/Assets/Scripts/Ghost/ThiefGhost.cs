using System;
using System.Collections;
using System.Linq.Expressions;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class ThiefGhost : MonoBehaviour
{
    [Header("Phase Settings")]
    [SerializeField] private int[] phaseHealth = { 20, 10, 20 };
    [SerializeField] private float[] stunDurations = { 5f, 10f, 5f };
    [SerializeField] private float[] flashlightThresholds = { 3f, 5f, 7f };

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
    private int currentHealth;
    private bool isAlive = true;
    private bool isStunned = false;
    private bool isInCooldown = false; // Cooldown status
    private bool canGetRoped = true;
    private bool isRoped = false;

    // UI Events
    public delegate void StunCooldownEvent(bool isCooldownActive);
    public event StunCooldownEvent OnStunCooldownChanged;

    public delegate void GhostSpawnEvent();
    public event GhostSpawnEvent OnGhostSpawned;
    private GameObject ghostAnchor;
    private Vector3 direction;
    [SerializeField] private float defaultSpeed = 5;
    [SerializeField] private float speed;



    void Start()
    {
        float angle = Random.Range(0, 2 * Mathf.PI);
        direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
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

        OnGhostSpawned?.Invoke(); // Notify UI that ghost has spawned
        StartPhase(0);
    }

    private void StartPhase(int phase)
    {
        if (phase >= phaseHealth.Length)
        {
            Die();
            return;
        }

        currentPhase = phase;
        currentHealth = phaseHealth[phase];
        isStunned = false;
        isInCooldown = false; // Reset cooldown on phase change

        Debug.Log($"Starting Phase {phase + 1} | HP: {currentHealth} | Stun Time: {stunDurations[phase]}s | Flashlight Threshold: {flashlightThresholds[phase]}s");

        ghostAnimator.Play("Float");
        PlayNextMovementAnimation();
    }

    private void PlayNextMovementAnimation()
    {

        // if (currentPhase < phaseAnimations.Length)
        // {
        //     string[] animations = phaseAnimations[currentPhase];

        //     if (animations.Length > 0)
        //     {
        //         string nextAnimation;
        //         do
        //         {
        //             nextAnimation = animations[Random.Range(0, animations.Length)];
        //         } while (nextAnimation == lastMovementAnimation);

        //         lastMovementAnimation = nextAnimation;
        //         movementAnimator.CrossFade(nextAnimation, 0.2f);
        //         Debug.Log($"Starting movement animation: {nextAnimation}");
        //     }
        // }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision detected with: {collision.gameObject.name} (Tag: {collision.gameObject.tag})");

        if (collision.gameObject.CompareTag("Rope") && canGetRoped && !isRoped)
        {
            if (isAlive)
            {
                isRoped = true;
                DestoryTrueParent(collision.gameObject);
                StartCoroutine(RopeDetectionTimer());
                GameObject rope = Instantiate(ropeOnGhost, transform.GetChild(0));
                rope.transform.position += new Vector3(-0.05f, 1f);
            }
        }
    }

    private void DestoryTrueParent(GameObject gameObject)
    {
        while (gameObject.transform.parent != null)
        {
            gameObject = gameObject.transform.parent.gameObject;
        }
        Destroy(gameObject, 0.1f);
    }

    IEnumerator RopeDetectionTimer()
    {
        canGetRoped = false;
        yield return new WaitForSeconds(1);
        canGetRoped = true;
    }

    public void StunGhost()
    {
        if (isStunned || isInCooldown)
        {
            return;
        }
        isStunned = true;
        direction = direction * -1;
        Debug.Log($"Ghost is stunned for {stunDurations[currentPhase]} seconds.");
        ghostAnimator.Play("Stun");
        speed = 0;
        Invoke(nameof(EndStun), 3f);
    }

    // private Vector3 RotateVector(Vector3 vector, float degree)
    // {
    //     float rad = degree * Mathf.Deg2Rad;
    //     float x = Mathf.Cos(rad * vector.x) - Mathf.Sin(rad * vector.z);
    //     float z = Mathf.Sin(rad * vector.x) + Mathf.Cos(rad * vector.z);
    //     return new Vector3(x, vector.y, z);
    // }

    private void EndStun()
    {
        isStunned = false;
        isInCooldown = true; // Enter cooldown phase
        Debug.Log("Ghost is no longer stunned. Entering cooldown.");

        movementAnimator.speed = 1;
        ghostAnimator.Play("Float");
        speed = defaultSpeed;
        OnStunCooldownChanged?.Invoke(true); // Notify UI that cooldown started
        Invoke(nameof(EndCooldown), .25f);
    }

    private void EndCooldown()
    {
        isInCooldown = false;
        Debug.Log("Cooldown ended. Ghost can now be stunned again.");

        OnStunCooldownChanged?.Invoke(false); // Notify UI that cooldown ended
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Ghost took {damage} damage. Remaining HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            AdvancePhase();
        }
    }

    private void AdvancePhase()
    {
        Debug.Log($"Phase {currentPhase + 1} completed.");
        StartPhase(currentPhase + 1);
    }

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

    public float GetHealthPercentage()
    {
        return Mathf.Clamp01((float)currentHealth / phaseHealth[currentPhase]);
    }

    void Update()
    {
        GhostMovement();
        // if (!isStunned && !isInCooldown)
        // {
        //     AnimatorStateInfo stateInfo = movementAnimator.GetCurrentAnimatorStateInfo(0);

        //     if (stateInfo.normalizedTime >= 1.0f && stateInfo.IsTag("Move"))
        //     {
        //         PlayNextMovementAnimation();
        //     }
        // }
    }

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

    // private void OldGhostMovement()
    // {
    //     if (!isStunned)
    //     {
    //         if (Vector3.Distance(ghostAnchor.transform.localPosition, currentMovementPattern[pathingIndex].getPoints()) < WRadius)
    //         {
    //             pathingIndex = (pathingIndex + movementDirection) % currentMovementPattern.Length;
    //         }
    //         ghostAnchor.transform.localPosition = Vector3.MoveTowards(ghostAnchor.transform.localPosition, currentMovementPattern[pathingIndex].getPoints(), CalculateMovementSpeed());
    //     }

    // }

    // private float CalculateMovementSpeed()
    // {
    //     Vector3 currPos = currentMovementPattern[pathingIndex].getPoints();
    //     Vector3 nextPos = currentMovementPattern[(pathingIndex + 1) % currentMovementPattern.Length].getPoints();
    //     float distance = Vector3.Distance(currPos, nextPos);
    //     return distance / currentMovementPattern[(pathingIndex + 1) % currentMovementPattern.Length].getFrames();
    // }

    // private struct Point
    // {
    //     Vector3 points;
    //     int frames;

    //     public Point(Vector3 points, int frames) : this()
    //     {
    //         this.points = points;
    //         this.frames = frames;
    //     }

    //     public Vector3 getPoints()
    //     {
    //         return points;
    //     }

    //     public int getFrames()
    //     {
    //         return frames;
    //     }
    // }
}
