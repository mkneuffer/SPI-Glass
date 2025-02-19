using UnityEngine;

public class Launcher : MonoBehaviour
{
    public Rigidbody _prefabWithRigidbody;
    private bool _canLaunch = true; // Track cooldown state
    [SerializeField] Vector3 spawningOffset = new Vector3();
    [SerializeField] Vector3 rotation = new Vector3();
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
        var x = spawningOffset.x * Camera.main.transform.right;
        var y = spawningOffset.y * Camera.main.transform.up;
        var z = spawningOffset.z * Camera.main.transform.forward;
        var newRot = rotation;
        newRot.Scale(Camera.main.transform.rotation.eulerAngles);
        var pos = Camera.main.transform.position + x + y + z;
        var forw = Camera.main.transform.forward;

        var thing = Instantiate(_prefabWithRigidbody, pos + (forw * 0.4f), Quaternion.Euler(rotation));
        thing.transform.LookAt(Camera.main.transform);
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
