using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseIcon : MonoBehaviour
{
    [SerializeField] GameObject icon;
    public float Speed = 1;
    public float originalSize;
    public float endSize;

    void Start()
    {
        originalSize = 1;
        endSize = originalSize * 2;
        pulseIcon();
    }

    public void pulseIcon()
    {
        while (icon.transform.localScale.x < endSize)
        {
            Debug.Log("growing");
            icon.transform.localScale *= Speed;
        }
        StartCoroutine(WaitForPulse());
    }

    IEnumerator WaitForPulse()
    {
        yield return new WaitForSeconds(1);
        while (icon.transform.localScale.x > originalSize)
        {
            Debug.Log("shrinking");
            icon.transform.localScale /= Speed;
        }
        pulseIcon();
    }
}
