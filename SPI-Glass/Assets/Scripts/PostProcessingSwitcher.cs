using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingSwitcher : MonoBehaviour
{
    [SerializeField] private Volume normalVolume; // Volume for normal world
    [SerializeField] private Volume ghostWorldVolume; // Volume for ghost world
    [SerializeField] private float transitionDuration = 2.0f;

    private Coroutine transitionCoroutine;

    private void Start()
    {
        // Ensure the ghost world volume is disabled initially
        ghostWorldVolume.weight = 0f;
    }

    public void EnterGhostWorld()
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(TransitionToGhostWorld());
    }

    private IEnumerator TransitionToGhostWorld()
    {
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;

            // Interpolate weights between normal and ghost world
            normalVolume.weight = Mathf.Lerp(1f, 0f, t);
            ghostWorldVolume.weight = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        // Ensure the final state is fully switched to ghost world
        normalVolume.weight = 0f;
        ghostWorldVolume.weight = 1f;
    }
}
