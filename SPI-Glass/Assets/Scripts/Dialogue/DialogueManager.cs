using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private Animator portraitAnimator;
    [SerializeField] private GameObject PortraitFrame;
    [SerializeField] private GameObject SpeakerFrame;
    [SerializeField] private PuzzleManager puzzleManager;
    [SerializeField] private MoveGhost ghostManager;
    [SerializeField] private InventoryManager InventoryManager;
    private Animator layoutAnimator;
    [SerializeField] private float automaticTextSpeedPerWord;
    [SerializeField] private Animator transition;
    [SerializeField] private float transitionTime;


    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;
    public Button grail;

    private Story currentStory;
    public bool isDialoguePlaying { get; private set; }

    private static DialogueManager instance;
    private bool notAutomatic;
    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUT_TAG = "layout";

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one DialogueManager in the scene");
        }
        instance = this;
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        isDialoguePlaying = false;
        dialoguePanel.SetActive(false);
        layoutAnimator = dialoguePanel.GetComponent<Animator>();
        notAutomatic = true;
        automaticTextSpeedPerWord = 0.5f;

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
        if (grail != null)
        {
            grail.onClick.AddListener(OnContinueClick); // For grail cutscene
        }

    }

    public void OnContinueClick()
    {
        if (currentStory.canContinue)
        {
            ContinueStory();
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private void Update()
    {
        if (!isDialoguePlaying)
        {
            return;
        }

        if (notAutomatic)
        {
            //Advance to next line on input
            List<Choice> currentChoices = currentStory.currentChoices;
            bool clicked = Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began);
            if (clicked && currentChoices.Count == 0)
            {
                ContinueStory();
            }
        }

    }



    //Start the dialogue
    //Call this function to run the dialogue
    //Parameters:
    //inkJSON: the .json file related to the .ink file of the text that will be played
    //notAutomatic: if true, player will need to click to advance the text, if false, text advances automatically
    public void EnterDialogueMode(TextAsset inkJSON, bool notAutomatic)
    {
        //If dialogue is already playing, return
        if (isDialoguePlaying)
        {
            return;
        }
        this.notAutomatic = notAutomatic;
        currentStory = new Story(inkJSON.text);
        isDialoguePlaying = true;
        dialoguePanel.SetActive(true);

        if (puzzleManager != null)
        {
            currentStory.BindExternalFunction("startPuzzle", (string puzzleName) =>
            {
                puzzleManager.setCanvasState(true);
            });
        }

        if (InventoryManager != null)
        {
            currentStory.BindExternalFunction("addItem", (string itemName) =>
            {
                ItemData item = InventoryManager.FindItemByName(itemName);
                if (item != null)
                {
                    InventoryManager.addItem(item);
                }
            });


            currentStory.BindExternalFunction("removeItem", (string itemName) =>
            {
                ItemData item = InventoryManager.FindItemByName(itemName);
                if (item != null)
                {
                    InventoryManager.RemoveItemByType(item);
                }
            });
        }

        if (ghostManager != null)
        {
            currentStory.BindExternalFunction("startFight", (string start) =>
            {
                ghostManager.startGhostFight();
            });
        }

        currentStory.BindExternalFunction("SpawnGhost", () =>
{
    XR_Placement placement = FindObjectOfType<XR_Placement>();
    if (placement != null)
    {
        placement.SpawnGhost();
    }
    else
    {
        Debug.LogWarning("XR_Placement script not found in the scene.");
    }
});
        
        currentStory.BindExternalFunction("DeleteGhost", () =>
        {
            XR_Placement placement = FindObjectOfType<XR_Placement>();
            if (placement != null)
            {
                placement.DeleteGhost(); // Call the method in XR_Placement
            }
            else
            {
                Debug.LogWarning("XR_Placement script not found in the scene.");
            }
        });
        
        currentStory.BindExternalFunction("SwitchToMap", () =>
        {
            StartCoroutine(LoadMapScene());
        });

        IEnumerator LoadMapScene()
        {
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }

        currentStory.BindExternalFunction("SwitchToScene1", () =>
        {
           StartCoroutine(LoadScene1());
        });

        IEnumerator LoadScene1()
        {
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        currentStory.BindExternalFunction("SwitchToScene2", () =>
        {
            StartCoroutine(LoadScene2());
        });

        IEnumerator LoadScene2()
        {
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene(2);
        }

        currentStory.BindExternalFunction("SwitchToObjectDetection", () =>
        {
            StartCoroutine(LoadObjectDetectionScene());
        });

        IEnumerator LoadObjectDetectionScene()
        {
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene(3);
        }

        currentStory.BindExternalFunction("SwitchToScene3", () =>
        {
            StartCoroutine(LoadScene3());
        });

        IEnumerator LoadScene3()
        {
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene(4);
        }

        currentStory.BindExternalFunction("SwitchToScene4", () =>
        {
            StartCoroutine(LoadScene4());
        });

        IEnumerator LoadScene4()
        {
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene(5);
        }

        currentStory.BindExternalFunction("SwitchToGhostFight", () =>
        {
            StartCoroutine(LoadGhostFight());
        });

        IEnumerator LoadGhostFight()
        {
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene(6);
        }

        currentStory.BindExternalFunction("SwitchToScene5", () =>
        {
            StartCoroutine(LoadScene5());
        });

        IEnumerator LoadScene5()
        {
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene(7);
        }

        currentStory.BindExternalFunction("SwitchToScene6", () =>
        {
            StartCoroutine(LoadScene6());
        });

        IEnumerator LoadScene6()
        {
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene(8);
        }


        //Reset display name, portrait and layout
        displayNameText.text = "???";
        portraitAnimator.Play("default");
        layoutAnimator.Play("right");
        if (this.notAutomatic == true)
        {
            ContinueStory();
        }
        else
        {
            StartCoroutine(AutomaticContinueStory(automaticTextSpeedPerWord));
        }

    }

    //Start the dialogue
    //Call this function to run the dialogue
    //Parameters:
    //inkJSON: the .json file related to the .ink file of the text that will be played
    //notAutomatic: if true, player will need to click to advance the text, if false, text advances automatically
    //automaticTextSpeed: how fast the automatic text updates
    public void EnterAutomaticDialogueMode(TextAsset inkJSON, bool notAutomatic, float automaticTextSpeedPerWord)
    {

        //If dialogue is already playing, return
        if (isDialoguePlaying)
        {
            return;
        }
        this.notAutomatic = notAutomatic;
        currentStory = new Story(inkJSON.text);
        isDialoguePlaying = true;
        dialoguePanel.SetActive(true);

        currentStory.BindExternalFunction("startPuzzle", (string puzzleName) =>
        {
            puzzleManager.setCanvasState(true);
        });

        //Reset display name, portrait and layout
        displayNameText.text = "???";
        portraitAnimator.Play("default");
        layoutAnimator.Play("left");

        StartCoroutine(AutomaticContinueStory(automaticTextSpeedPerWord));

    }

    //Leave the dialogue
    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.05f);
        if (puzzleManager != null)
        {
            currentStory.UnbindExternalFunction("startPuzzle");
        }
        
        if (ghostManager != null)
        {
            currentStory.UnbindExternalFunction("startFight");
        }
        

        isDialoguePlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    //Goes to next line in the diaglogue
    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
            DisplayChoices();
            HandleTags(currentStory.currentTags);
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private IEnumerator AutomaticContinueStory(float automaticTextSpeedPerWord)
    {
        if (currentStory.canContinue)
        {

            string currentText = currentStory.Continue();
            dialogueText.text = currentText;
            HandleTags(currentStory.currentTags);

            //Give error if trying to do choices
            if (currentStory.currentChoices.Count > 0)
            {
                Debug.LogError("Automatic Dialogue does not support choices");
            }
            //Disables all choices
            for (int i = 0; i < choices.Length; i++)
            {
                choices[i].gameObject.SetActive(false);
            }


            //Calculate autotextspeed
            float textSpeed = 1.0f + (GetWordCount(currentText) * automaticTextSpeedPerWord);
            yield return new WaitForSeconds(textSpeed);

            StartCoroutine(AutomaticContinueStory(automaticTextSpeedPerWord));
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    //Given an input string, return the number of words in that string
    private float GetWordCount(string input)
    {
        float count = 1.0f;
        foreach (char c in input)
        {
            if (c == ' ')
            {
                count++;
            }
        }
        return count;
    }

    //Given the current dialogue's tags, do the things we need to do
    private void HandleTags(List<string> currentTags)
    {
        //Loop through each tag
        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag could not be appropriately parsed: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            //Does stuff with the tags
            switch (tagKey)
            {
                case SPEAKER_TAG:
                    if (tagValue != "0")
                    {
                        SpeakerFrame.SetActive(true);
                        displayNameText.text = tagValue;
                    }
                    else
                    {
                        SpeakerFrame.SetActive(false);
                    }

                    break;

                case PORTRAIT_TAG:
                    if (tagValue != "0")
                    {
                        PortraitFrame.SetActive(true);
                        portraitAnimator.Play(tagValue);
                    }
                    else
                    {
                        PortraitFrame.SetActive(false);
                    }

                    break;

                case LAYOUT_TAG:
                    if (!notAutomatic)
                    {
                        if (tagValue == "left")
                        {
                            layoutAnimator.Play("left-noninterrupt");
                        }
                        else
                        {
                            layoutAnimator.Play(tagValue);
                        }
                    }
                    else
                    {
                        layoutAnimator.Play(tagValue);
                    }
                    break;

                case "add_item":
                    ItemData itemToAdd = InventoryManager.FindItemByName(tagValue);
                    if (itemToAdd != null)
                    {
                        InventoryManager.addItem(itemToAdd);
                    }
                    break;

                case "remove_item":
                    ItemData itemToRemove = InventoryManager.FindItemByName(tagValue);
                    if (itemToRemove != null)
                    {
                        InventoryManager.addItem(itemToRemove);
                    }
                    break;

                default:
                    Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }

    //Display the list of all choices
    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        //Check if the UI can support the number of choices coming in
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI can support. Number of choices given: " + currentChoices.Count + ", Number of choices supported: " + choices.Length);
        }

        int index = 0;
        // enable and initialize the choices up to the amount of choies for this line of dialogue
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        // go through the remaining choices the UI supports and make sure they're hidden
        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }
    }


    //Connected to the onclick funciton of the buttons, chooses the choice of the button clicked
    public void MakeChoice(int choiceIndex)
    {
        EventSystem.current.SetSelectedGameObject(choices[choiceIndex].gameObject);
        currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueStory();
    }

   public void resetGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

}


