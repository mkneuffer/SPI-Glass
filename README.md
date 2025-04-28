# SPI-Glass Documentation

Welcome to the SPI-Glass documentation. Here you’ll find an overview of our core systems: AR tracking, dialogue, and wood-collection via semantic segmentation.

---

## Table of Contents

1. [AR Tracking with **StumpPlace**](#ar-tracking-with-stumpplace)  
2. [Dialogue System](#dialogue-system)  
3. [Semantic Segmentation for Wood Collection](#semantic-segmentation-for-wood-collection)  

---

## Videos

- **Full Playthrough**: [Watch here](<https://www.youtube.com/watch?v=oYqmurLRoHE>)
- **Game Trailer**: [Watch here](https://youtube.com/shorts/QzAqoT1zwnc)

---

## AR Tracking with **StumpPlace**

The **StumpPlace** component encapsulates all AR plane detection and object-placement logic, built on top of Unity’s AR Foundation (or Niantic Lightship ARDK). It:

- **Listens** for user input (touch or click).
- **Performs** an AR raycast against detected horizontal surfaces.
- **Instantiates** a stump prefab at the hit location.
- **Creates** and **manages** AR anchors so that stumps stay locked to the real-world position as tracking data updates.
- **Distance Filtering**: Configurable `minDistance` and `maxDistance` settings ensure stumps are only placed within the desired range from the user’s viewpoint.

<details>
<summary>Example: StumpPlace.cs (simplified)</summary>

```csharp
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class StumpPlace : MonoBehaviour
{
    [SerializeField] private GameObject stumpPrefab;
    [SerializeField] private float minDistance = 0.5f;
    [SerializeField] private float maxDistance = 3.0f;
    private ARRaycastManager raycastManager;

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        if (Input.touchCount == 0) return;
        var touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;

        if (raycastManager.Raycast(touch.position, out var hits, TrackableType.PlaneWithinPolygon))
        {
            var pose = hits[0].pose;
            float distance = Vector3.Distance(Camera.main.transform.position, pose.position);
            if (distance < minDistance || distance > maxDistance) return;

            var stump = Instantiate(stumpPrefab, pose.position, pose.rotation);
            stump.AddComponent<ARAnchor>();
        }
    }
}
```
</details>

---

## Dialogue System

We leverage [Inkle’s Ink](https://www.inklestudios.com/ink/) for a flexible, branching narrative:

1. **Script files** live under `Assets/Dialogues/*.ink`.
2. At runtime, `DialogueManager`  
   - **Loads** the chosen `.ink.json` via Unity’s `TextAsset`.  
   - **Advances** text with `story.Continue()`.  
   - **Presents** choices as UI buttons.  
   - **Handles** player selections, feeding them back into `story.ChooseChoiceIndex(...)`.
3. **Callbacks** trigger in-game events (e.g. spawning objects, playing sounds).

<details>
<summary>Key excerpts from DialogueManager.cs</summary>

```csharp
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextAsset inkJSON;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject choicesContainer;
    [SerializeField] private Button choiceButtonPrefab;

    private Story story;

    public void StartDialogue()
    {
        story = new Story(inkJSON.text);
        DisplayNextLine();
    }

    void DisplayNextLine()
    {
        if (!story.canContinue) { EndDialogue(); return; }

        dialogueText.text = story.Continue();
        CreateChoiceButtons();
    }

    void CreateChoiceButtons()
    {
        foreach (Transform t in choicesContainer.transform) Destroy(t.gameObject);

        for (int i = 0; i < story.currentChoices.Count; i++)
        {
            var choice = story.currentChoices[i];
            var btn = Instantiate(choiceButtonPrefab, choicesContainer.transform);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
            int index = i;
            btn.onClick.AddListener(() => OnChoiceSelected(index));
        }
    }

    void OnChoiceSelected(int index)
    {
        story.ChooseChoiceIndex(index);
        DisplayNextLine();
    }

    void EndDialogue() { /* Cleanup and signal end of dialogue */ }
}
```
</details>

---

## Semantic Segmentation for Wood Collection

To detect and collect “wood” in the player’s environment, we integrate a custom semantic-segmentation model via Unity Barracuda:

1. **Camera feed** frames are passed to the **SegmentationRunner**, which  
   - **Preprocesses** the image into a tensor.  
   - **Runs** inference on our trained “wood detector” model.  
   - **Returns** a binary mask highlighting wood-pixels.
2. On **touch input**, we sample the mask at the tapped pixel.  
   - **If** that pixel belongs to wood, we spawn a “wood fragment” collectible at the corresponding world raycast hit.
3. This pipeline ensures the user only collects actual wood surfaces, even in complex real-world scenes.

<details>
<summary>Pseudocode for segmentation in WoodCollector.cs</summary>

```csharp
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class WoodCollector : MonoBehaviour
{
    [SerializeField] private NNModel segmentationModel;
    [SerializeField] private GameObject woodPrefab;
    private IWorker worker;
    private ARCameraManager cameraManager;

    void Start()
    {
        var model = ModelLoader.Load(segmentationModel);
        worker = WorkerFactory.CreateWorker(model);
        cameraManager = FindObjectOfType<ARCameraManager>();
    }

    void Update()
    {
        if (Input.touchCount == 0 || Input.GetTouch(0).phase != TouchPhase.Began) return;

        cameraManager.TryAcquireLatestCpuImage(out var image);
        var mask = RunSegmentation(image);
        image.Dispose();

        if (mask.IsWoodPixel(touchX, touchY))
        {
            Instantiate(woodPrefab, hitPose.position, Quaternion.identity);
        }
    }

    private Tensor RunSegmentation(XRCpuImage image)
    {
        // Convert image to Tensor, dispatch to worker, retrieve mask tensor
    }

    void OnDestroy() => worker.Dispose();
}
```
</details>

---

> **Next Steps:**  
> - Tweak AR surface filters in `StumpPlace` to ignore overly small planes.  
> - Extend Ink scripts to support inventory checks for collected wood.  
> - Retrain segmentation model with more wood-type samples for robustness.

---

*Happy coding!*  
*— SPI-Glass Team*

