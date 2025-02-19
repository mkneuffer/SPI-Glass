using UnityEngine;

public class Launcher : MonoBehaviour
{
    public Rigidbody _prefabWithRigidbody;
    private bool _canLaunch = true; // Track cooldown state
    [SerializeField] float ySpawningOffset = 0;
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) && _canLaunch)
#else
        if (Input.touchCount > 0 && _canLaunch)
#endif
        {
            LaunchBall();
        }
    }

    private void LaunchBall()
    {
        // spawn in front of the camera
        var pos = Camera.main.transform.position + new Vector3(0, ySpawningOffset);
        var forw = Camera.main.transform.forward;
        var thing = Instantiate(_prefabWithRigidbody, pos + (forw * 0.4f), Quaternion.identity);

        thing.AddForce(forw * 800.0f);

        // Destroy the object after 5 seconds
        Destroy(thing.gameObject, 5.0f);

        // Start cooldown
        StartCoroutine(LaunchCooldown());
    }

    private System.Collections.IEnumerator LaunchCooldown()
    {
        _canLaunch = false;
        yield return new WaitForSeconds(1.0f); // 1-second cooldown
        _canLaunch = true;
    }
}
