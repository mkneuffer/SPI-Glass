using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PinManager : MonoBehaviour
{
    public static PinManager Instance;
    [SerializeField] private Animator transition;
    
    private int lockedPins = 0;
    private int totalPins = 5;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void LockPin()
    {
        lockedPins++;
        Debug.Log($"Pin locked! {lockedPins}/{totalPins} locked.");

        if (lockedPins >= totalPins)
        {
            Debug.Log("All pins locked! Transitioning to next scene.");
            StartCoroutine(LoadNextScene());
        }
    }

    public void ResetAllPins()
    {
        Debug.Log("Resetting all pins.");
        lockedPins = 0; // Reset the locked pins counter

        // Find and reset all pins
        PinInteraction[] pins = FindObjectsOfType<PinInteraction>();
        foreach (var pin in pins)
        {
            pin.StartCoroutine(pin.ResetPinCoroutine());
        }
    }

    private IEnumerator LoadNextScene()
    {
        if (transition != null)
        {
            transition.SetTrigger("Start");
        }

        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene("Thief Scene 5");
    }
}