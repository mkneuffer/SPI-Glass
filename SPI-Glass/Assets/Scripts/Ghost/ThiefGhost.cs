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

        StartPhase(0);
    }

    private void StartPhase(int phase)
    {
        if (phase >= totalPhases)
        {
            Die();
            return;
        }

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

        ghostAnimator.Play("Float");
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

    private void DestoryTrueParent(GameObject gameObject)
    {
        while (gameObject.transform.parent != null)
        {
            gameObject = gameObject.transform.parent.gameObject;
        }
        Destroy(gameObject, 0.2f);
    }

    public void StunGhost()
    {
        if (isStunned || isInCooldown)
        {
            return;
        }
        isStunned = true;
        direction = direction * -1;
        Debug.Log($"Ghost is stunned.");
        ghostAnimator.Play("Stun");
        speed = 0;
        Invoke(nameof(EndStun), 3f);
    }

    private void EndStun()
    {
        if (!trapped)
        {
            isStunned = false;
            isInCooldown = true; // Enter cooldown phase
            Debug.Log("Ghost is no longer stunned. Entering cooldown.");

            ghostAnimator.Play("Float");

            speed = defaultSpeed;
            Invoke(nameof(EndCooldown), .1f);
        }
    }

    private void EndCooldown()
    {
        isInCooldown = false;
        Debug.Log("Cooldown ended. Ghost can now be stunned again.");

        OnStunCooldownChanged?.Invoke(false); // Notify UI that cooldown ended
    }

    public void UpdateTrapped(bool isTrapped)
    {
        if (trapped == isTrapped)
        {
            return;
        }
        trapped = isTrapped;
        if (isTrapped)
        {
            speed = 0;
        }
        else if (!isStunned)
        {
            speed = defaultSpeed;
        }
        Debug.Log($"Trapped is set to {trapped}");
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

    void Update()
    {
        ghostModel.transform.rotation = Quaternion.LookRotation(direction);
        ghostModel.transform.Rotate(new Vector3(0, -90));
        GhostMovement();
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
}
