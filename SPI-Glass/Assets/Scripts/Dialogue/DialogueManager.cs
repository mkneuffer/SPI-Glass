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
    [SerializeField] private GameObject nameField;
    [SerializeField] private SettingsSaving settingsSaving;
    private Animator layoutAnimator;
    [SerializeField] private Animator transition;
    [SerializeField] private float transitionTime;
    [SerializeField] private PostProcessingSwitcher postProcessingSwitcher;
    [SerializeField] private TextAsset scene4Prompt;
    [SerializeField] private TestTriggerDialogue triggerDialogue;



    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;
    public Button grail;

    private Story currentStory;
    public bool isDialoguePlaying { get; private set; }

    private static DialogueManager instance;
    private bool manuallyIncrement;
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
        manuallyIncrement = true;

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

        if (manuallyIncrement)
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
    //automaticTextSpeed: how fast the automatic text updates, default = 0.5f
    public void EnterDialogueMode(TextAsset inkJSON, bool manuallyIncrement, float automaticTextSpeedPerWord = 0.5f)
    {
        //If dialogue is already playing, return
        if (isDialoguePlaying)
        {
            return;
        }
        this.manuallyIncrement = manuallyIncrement;
        currentStory = new Story(inkJSON.text);
        isDialoguePlaying = true;
        dialoguePanel.SetActive(true);

        BindDialogueFunctions();

        //Reset display name, portrait and layout
        displayNameText.text = "???";
        portraitAnimator.Play("default");
        layoutAnimator.Play("left");
        if (this.manuallyIncrement == true)
        {
            ContinueStory();
        }
        else
        {
            StartCoroutine(AutomaticContinueStory(automaticTextSpeedPerWord));
        }

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
            //formula = 1 + wordCount * speed
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
                    if (tagValue == "0")
                    {
                        SpeakerFrame.SetActive(false);
                    }
                    else if (tagValue == "Player")
                    {
                        displayNameText.text = "You";
                    }
                    //No portrait
                    else
                    {
                        SpeakerFrame.SetActive(true);
                        displayNameText.text = tagValue;
                    }
                    break;

                case PORTRAIT_TAG:
                    if (tagValue == "0")
                    {
                        PortraitFrame.SetActive(false);
                    }
                    //No portrait
                    else
                    {
                        PortraitFrame.SetActive(true);
                        portraitAnimator.Play(tagValue);
                    }

                    break;

                case LAYOUT_TAG:
                    if (!manuallyIncrement)
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
        Debug.Log("Making choice: " + choiceIndex);
        EventSystem.current.SetSelectedGameObject(choices[choiceIndex].gameObject);
        currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueStory();
    }

    public void resetGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    //Binds the functions written in the ink file to functions in the code
    private void BindDialogueFunctions()
    {

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

        if (nameField != null)
            Debug.Log("name field detected");
        {
            currentStory.BindExternalFunction("openNameField", () =>
            {
                nameField.SetActive(true);
            });
        }

        {
            currentStory.BindExternalFunction("scene4Func", () =>
            {
                Debug.Log("scene 4 func started");
                if (scene4Prompt != null)
                {
                    EnterDialogueMode(scene4Prompt, false, 0.6f);
                }
                else
                {
                    Debug.LogWarning("no scene 4 prompt found");
                }

                XR_Placement placement = FindObjectOfType<XR_Placement>();
                if (placement != null)
                {
                    placement.SpawnGhost(); // Makes the prefab visible
                }
                else
                {
                    Debug.LogWarning("XR_Placement script not found in the scene.");
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
                placement.SpawnGhost(); // Call the method in XR_Placement
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
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Map1");
        }

        currentStory.BindExternalFunction("SwitchToScene1", () =>
        {
            StartCoroutine(LoadScene1());
        });

        IEnumerator LoadScene1()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Scene1");
        }

        currentStory.BindExternalFunction("SwitchToScene2", () =>
        {
            StartCoroutine(LoadScene2());
        });

        IEnumerator LoadScene2()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Scene2");
        }

        currentStory.BindExternalFunction("SwitchToObjectDetection", () =>
        {
            StartCoroutine(LoadObjectDetectionScene());
        });

        IEnumerator LoadObjectDetectionScene()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("ObjectDetection");
        }

        currentStory.BindExternalFunction("SwitchToScene3", () =>
        {
            StartCoroutine(LoadScene3());
        });

        IEnumerator LoadScene3()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Scene3");
        }

        currentStory.BindExternalFunction("SwitchToScene4", () =>
        {
            StartCoroutine(LoadScene4());
        });

        IEnumerator LoadScene4()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Scene4");
        }

        currentStory.BindExternalFunction("SwitchToGhostFight", () =>
        {
            StartCoroutine(LoadGhostFight());
        });

        IEnumerator LoadGhostFight()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Stump 1");
        }

        currentStory.BindExternalFunction("SwitchToScene5", () =>
        {
            StartCoroutine(LoadScene5());
        });

        IEnumerator LoadScene5()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Scene5");
        }

        currentStory.BindExternalFunction("SwitchToScene6", () =>
        {
            StartCoroutine(LoadScene6());
        });

        IEnumerator LoadScene6()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Scene6");
        }

        currentStory.BindExternalFunction("SwitchToSceneThief1", () =>
        {
            StartCoroutine(LoadSceneThief1());
        });

        IEnumerator LoadSceneThief1()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Scene 1");
        }

        currentStory.BindExternalFunction("SwitchToSceneThief2", () =>
        {
            StartCoroutine(LoadSceneThief2());
        });

        IEnumerator LoadSceneThief2()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Scene 2");
        }

        currentStory.BindExternalFunction("SwitchToSceneThief3", () =>
        {
            StartCoroutine(LoadSceneThief3());
        });

        IEnumerator LoadSceneThief3()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Scene 3");
        }

        currentStory.BindExternalFunction("SwitchToSceneThief4", () =>
        {
            StartCoroutine(LoadSceneThief4());
        });

        IEnumerator LoadSceneThief4()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Scene 4");
        }

        currentStory.BindExternalFunction("SwitchToSceneThief5", () =>
        {
            StartCoroutine(LoadSceneThief5());
        });

        IEnumerator LoadSceneThief5()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Scene 5");
        }

        currentStory.BindExternalFunction("SwitchToSceneThief6", () =>
        {
            StartCoroutine(LoadSceneThief6());
        });

        IEnumerator LoadSceneThief6()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Scene 6");
        }

        currentStory.BindExternalFunction("SwitchToSceneThief7", () =>
        {
            StartCoroutine(LoadSceneThief7());
        });

        IEnumerator LoadSceneThief7()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Scene 7");
        }

        currentStory.BindExternalFunction("SwitchToSceneThief8", () =>
        {
            StartCoroutine(LoadSceneThief8());
        });

        IEnumerator LoadSceneThief8()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Scene 8");
        }

        currentStory.BindExternalFunction("SwitchToSceneThief9", () =>
        {
            StartCoroutine(LoadSceneThief9());
        });

        IEnumerator LoadSceneThief9()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Scene 9");
        }

        currentStory.BindExternalFunction("SwitchToSceneThief10", () =>
        {
            StartCoroutine(LoadSceneThief10());
        });

        IEnumerator LoadSceneThief10()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Scene 10");
        }

        currentStory.BindExternalFunction("SwitchToSceneThief11", () =>
        {
            StartCoroutine(LoadSceneThief11());
        });

        IEnumerator LoadSceneThief11()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Scene 11");
        }




        currentStory.BindExternalFunction("SwitchToMapInterlude", () =>
        {
            StartCoroutine(LoadSceneMapInterlude());
        });

        IEnumerator LoadSceneMapInterlude()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Map Interlude");
        }




        // For Thief Scenes
        currentStory.BindExternalFunction("SwitchToThiefScene1", () =>
        {
            StartCoroutine(LoadThiefScene1());
        });

        IEnumerator LoadThiefScene1()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Scene 1");
        }



        currentStory.BindExternalFunction("SwitchToThiefScene2", () =>
        {
            StartCoroutine(LoadThiefScene2());
        });

        IEnumerator LoadThiefScene2()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Scene 2");
        }



        currentStory.BindExternalFunction("SwitchToThiefScene3", () =>
        {
            StartCoroutine(LoadThiefScene3());
        });

        IEnumerator LoadThiefScene3()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Scene 3");
        }




        currentStory.BindExternalFunction("SwitchToThiefScene4", () =>
        {
            StartCoroutine(LoadThiefScene4());
        });

        IEnumerator LoadThiefScene4()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Scene 4");
        }




        currentStory.BindExternalFunction("SwitchToLockpicking", () =>
        {
            StartCoroutine(LoadThiefLockpicking());
        });

        IEnumerator LoadThiefLockpicking()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Lockpicking");
        }




        currentStory.BindExternalFunction("SwitchToThiefScene5", () =>
        {
            StartCoroutine(LoadThiefScene5());
        });

        IEnumerator LoadThiefScene5()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Scene 5");
        }





        currentStory.BindExternalFunction("SwitchToBlackjack", () =>
        {
            StartCoroutine(LoadThiefBlackjack());
        });

        IEnumerator LoadThiefBlackjack()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Blackjack");
        }




        currentStory.BindExternalFunction("SwitchToThiefScene6", () =>
        {
            StartCoroutine(LoadThiefScene6());
        });

        IEnumerator LoadThiefScene6()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Scene 6");
        }





        currentStory.BindExternalFunction("SwitchToThiefScene7", () =>
        {
            StartCoroutine(LoadThiefScene7());
        });

        IEnumerator LoadThiefScene7()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Scene 7");
        }




        currentStory.BindExternalFunction("SwitchToThiefScene8", () =>
        {
            StartCoroutine(LoadThiefScene8());
        });

        IEnumerator LoadThiefScene8()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Scene 8");
        }





        currentStory.BindExternalFunction("SwitchToThiefFight", () =>
        {
            StartCoroutine(LoadThiefFight());
        });

        IEnumerator LoadThiefFight()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("ThiefFight");
        }




        currentStory.BindExternalFunction("SwitchToThiefScene9", () =>
        {
            StartCoroutine(LoadThiefScene9());
        });

        IEnumerator LoadThiefScene9()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Scene 9");
        }




        currentStory.BindExternalFunction("SwitchToThiefScene10", () =>
        {
            StartCoroutine(LoadThiefScene10());
        });

        IEnumerator LoadThiefScene10()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Scene 10");
        }




        currentStory.BindExternalFunction("SwitchToThiefScene11", () =>
        {
            StartCoroutine(LoadThiefScene11());
        });

        IEnumerator LoadThiefScene11()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Scene 11");
        }

        currentStory.BindExternalFunction("SwitchToThiefMap1", () =>
        {
            StartCoroutine(LoadThiefMap1());
        });

        IEnumerator LoadThiefMap1()
        {
            Debug.Log("loading thief map 1");
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            Debug.Log("second load");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Map 1");
        }

        currentStory.BindExternalFunction("SwitchToThiefMap2", () =>
        {
            StartCoroutine(LoadThiefMap2());
        });

        IEnumerator LoadThiefMap2()
        {
            dialoguePanel.SetActive(false);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Thief Map 2");
        }




        // Prefabs
        currentStory.BindExternalFunction("makePrefabVisible", () =>
    {
        XR_Placement placement = FindObjectOfType<XR_Placement>();
        if (placement != null)
        {
            placement.SpawnGhost(); // Makes the prefab visible
        }
        else
        {
            Debug.LogWarning("XR_Placement script not found in the scene.");
        }
    });

        currentStory.BindExternalFunction("EnterGhostWorld", () =>
            {
                if (postProcessingSwitcher != null)
                {
                    postProcessingSwitcher.EnterGhostWorld();
                }
                else
                {
                    Debug.LogError("PostProcessingSwitcher is not assigned in DialogueManager.");
                }
            });

    }

}


