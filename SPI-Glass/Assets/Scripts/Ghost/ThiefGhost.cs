using System;
using System.Collections;
using System.Linq.Expressions;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class ThiefGhost : MonoBehaviour
{
    [Header("Phase Settings")]
    [SerializeField] private int[] phaseHealth = { 20, 10, 20 };
    [SerializeField] private float[] stunDurations = { 5f, 10f, 5f };
    [SerializeField] private float[] flashlightThresholds = { 3f, 5f, 7f };

    [Header("Cooldown Settings")]
    [SerializeField] private float stunCooldownDuration = 3f; // 3 seconds of immunity after stun

    [Header("References")]
    [SerializeField] private Animator ghostAnimator;
    [SerializeField] private Animator movementAnimator;
    [SerializeField] private Rigidbody ghostRigidbody;
    [SerializeField] private Collider selectedCollider;

    [Header("Fog Animation & Scene Transition")]
    [SerializeField] private GameObject fogObject; // Assign fog object in the Inspector
    [SerializeField] private string nextSceneName; // Assign next scene in the Inspector

    [Header("Movement Animations")]
    [SerializeField]
    private string[][] phaseAnimations =
    {
        new string[] { "Phase1AThief", "Phase1BThief", "Phase1CThief" },
        new string[] { "Phase2AThief", "Phase2BThief", "Phase2CThief" },
        new string[] { "Phase3AThief", "Phase3BThief", "Phase3CThief" }
    };

    [Header("Rope Stuff")]
    [SerializeField] private GameObject ropeOnGhost;

    private int currentPhase = 0;
    private int currentHealth;
    private bool isAlive = true;
    private bool isStunned = false;
    private bool isInCooldown = false; // Cooldown status
    private bool canGetRoped = true;
    private bool isRoped = false;
    private float flashlightTimer = 0f;
    private int movementDirection = 1; //1=forwards -1=backwards 0=stopped

    private string lastMovementAnimation = "";

    // UI Events
    public delegate void StunCooldownEvent(bool isCooldownActive);
    public event StunCooldownEvent OnStunCooldownChanged;

    public delegate void GhostSpawnEvent();
    public event GhostSpawnEvent OnGhostSpawned;

    private Point[] phase1A = new Point[]{
    new Point(new Vector3(0.3919599f, 0, 0.3480599f), 190),
    new Point(new Vector3(-0.6354175f, 0, -0.1155693f), 308),
    new Point(new Vector3(0.03632307f, 0, 1.791747f), 607),
    new Point(new Vector3(0.1553136f, 0, 0.1605787f), 906),
    new Point(new Vector3(0.30706f, 0, 0.34559f), 1060),
    new Point(new Vector3(0.1312969f, 0, 0.1312969f), 1209)};

    private int pathingIndex = 0;
    private Point[] currentMovementPattern;
    private float WRadius = .05f;
    private GameObject ghostAnchor;
    private Vector3 direction;
    [SerializeField] private float speed;



    void Start()
    {
        int angle = Random.Range(0, 360);
        direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        speed = 5;
        ghostAnchor = transform.GetChild(0).gameObject;
        currentMovementPattern = phase1A;
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
        pathingIndex = 0;
        if (phase >= phaseHealth.Length)
        {
            Die();
            return;
        }

        currentPhase = phase;
        currentHealth = phaseHealth[phase];
        isStunned = false;
        isInCooldown = false; // Reset cooldown on phase change
        flashlightTimer = 0f;

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

        if (collision.gameObject.CompareTag("Ball"))
        {
            if (isStunned && isAlive)
            {
                Debug.Log("Ghost is stunned. Taking 2 damage.");
                TakeDamage(2);
                Destroy(collision.gameObject, 0.1f);
            }
        }
        else if (collision.gameObject.CompareTag("Rope") && canGetRoped && !isRoped)
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

    public void HandleFlashlight()
    {
        if (isStunned || !isAlive || isInCooldown)
        {
            Debug.Log("Flashlight hit ghost but it is stunned, dead, or in cooldown. No effect.");
            return;
        }

        flashlightTimer += Time.deltaTime;
        Debug.Log($"Flashlight on ghost: {flashlightTimer:F2}s");

        if (flashlightTimer >= flashlightThresholds[currentPhase])
        {
            StunGhost();
        }
    }

    public void StunGhost()
    {
        isStunned = true;

        Debug.Log($"Ghost is stunned for {stunDurations[currentPhase]} seconds.");
        ghostAnimator.Play("Stun");
        Invoke(nameof(EndStun), stunDurations[currentPhase]);
    }

    private void EndStun()
    {
        isStunned = false;
        isInCooldown = true; // Enter cooldown phase
        Debug.Log("Ghost is no longer stunned. Entering cooldown.");

        movementAnimator.speed = 1;
        ghostAnimator.Play("Float");

        OnStunCooldownChanged?.Invoke(true); // Notify UI that cooldown started
        movementDirection *= -1;
        if (movementDirection == -1) //reverse
        {

        }
        Invoke(nameof(EndCooldown), stunCooldownDuration);
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

    public float GetFlashlightProgress()
    {
        return isInCooldown ? 0 : Mathf.Clamp01(flashlightTimer / flashlightThresholds[currentPhase]);
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
            direction = Vector3.Reflect(direction, new Vector3(-1, 0, 0));
        }
        else if (ghostAnchor.transform.localPosition.x < -5)
        {
            direction = Vector3.Reflect(direction, new Vector3(1, 0, 0));
        }
        if (ghostAnchor.transform.localPosition.z > 5)
        {
            direction = Vector3.Reflect(direction, new Vector3(0, 0, 1));
        }
        else if (ghostAnchor.transform.localPosition.z < -5)
        {
            direction = Vector3.Reflect(direction, new Vector3(0, 0, -1));
        }
        ghostAnchor.transform.localPosition += direction * speed * Time.deltaTime;

    }

    private void OldGhostMovement()
    {
        if (!isStunned)
        {
            if (Vector3.Distance(ghostAnchor.transform.localPosition, currentMovementPattern[pathingIndex].getPoints()) < WRadius)
            {
                pathingIndex = (pathingIndex + movementDirection) % currentMovementPattern.Length;
            }
            ghostAnchor.transform.localPosition = Vector3.MoveTowards(ghostAnchor.transform.localPosition, currentMovementPattern[pathingIndex].getPoints(), CalculateMovementSpeed());
        }

    }

    private float CalculateMovementSpeed()
    {
        Vector3 currPos = currentMovementPattern[pathingIndex].getPoints();
        Vector3 nextPos = currentMovementPattern[(pathingIndex + 1) % currentMovementPattern.Length].getPoints();
        float distance = Vector3.Distance(currPos, nextPos);
        return distance / currentMovementPattern[(pathingIndex + 1) % currentMovementPattern.Length].getFrames();
    }

    private struct Point
    {
        Vector3 points;
        int frames;

        public Point(Vector3 points, int frames) : this()
        {
            this.points = points;
            this.frames = frames;
        }

        public Vector3 getPoints()
        {
            return points;
        }

        public int getFrames()
        {
            return frames;
        }
    }
}
