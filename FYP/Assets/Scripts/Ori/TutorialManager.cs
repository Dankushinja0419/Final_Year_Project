using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField] private HiraganaChecker hiraganaChecker; // Reference to the HiraganaChecker script
    [SerializeField] private LevelManager levelManager; // Reference to LevelManager Script
    [SerializeField] public ScreenFader screenFader; // Reference to ScreenFader script

    [Header("Reset Difficulty")]
    [SerializeField] private GameData difficultyData;

    [Header("Buttons")]
    [SerializeField] private Button skipTutorialButton;
    [SerializeField] private Button checkButton;
    [SerializeField] private Button giveUpButton;
    [SerializeField] private Button skipLevelButton; 
    [SerializeField] private Button clearButton;

    [Header("Drawings")]
    [SerializeField] Button[] hiraganaButtons;
    [SerializeField] private List<GameObject> drawings;

    [Header("Arrows")]
    [SerializeField] private List<GameObject> arrows;

    [Header("Other Objects")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject endTutorialPanel;
    [SerializeField] private GameObject hiraganaButtonPanel;
    [SerializeField] private GameObject tutorialTextPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;

    [SerializeField] public InputActionProperty buttonA;
    [SerializeField] public InputActionProperty buttonB;

    private string[] tutorialSentences;

    private int currentSentenceIndex = 0;
    private bool canPress = true;
    private float pressInterval = 0.5f;

    private int actionIdx = 1;

    private bool nextAction = false;

    private bool previousActionDone = false;
    private bool previousDialogDone = false;
    private bool setDelayPanel = true;
    private bool tutorialButtonClicked = false;
    private bool checkDrawing = false;


    // Start is called before the first frame update
    void Start()
    {
        skipTutorialButton.onClick.AddListener(SkipTutorial);
        checkButton.onClick.AddListener(TutorialbuttonClicked);
        clearButton.onClick.AddListener(ClearPictures);

        for (int i = 0; i < hiraganaButtons.Length; i++)
        {
            int index = i; // Capture the current index for the lambda
            hiraganaButtons[i].onClick.AddListener(() => ShowDrawing(index));
        }

        checkButton.interactable = false;
        giveUpButton.interactable = false;
        skipLevelButton.interactable = false;
        hiraganaButtonPanel.SetActive(false);
        
        tutorialTextPanel.SetActive(true);
        Action1();
    }

    // Update is called once per frame
    void Update() 
    {
        if (buttonA.action.triggered && canPress)
        {
            ShowNextSentence();
            canPress = false;
            StartCoroutine(PressInterval(pressInterval));
        }

        if (tutorialTextPanel.activeSelf == false)
        {
            canPress = false;
        }
        else
        {
            canPress = true;
        }

        if (nextAction)
        {
            actionIdx++;
            currentSentenceIndex = 0;
            nextAction = false;
        }

        CheckAction(actionIdx);

        if (levelManager.gameLevel > 1)
        {
            EndTutorial();
        }
    }

    private void OnEnable()
    {
        buttonA.action.Enable();
        buttonB.action.Enable();
    }

    private void OnDisnable()
    {
        buttonA.action.Enable();
        buttonB.action.Enable();
    }

    private void ShowDrawing(int index)
    {
        // Activate the selected object and deactivate the rest
        for (int i = 0; i < drawings.Count; i++)
        {
            if (i == index)
            {
                drawings[i].SetActive(true); // Activate the selected object.               
            }
            else
            {
                drawings[i].SetActive(false); // Deactivate all others.
            }
        }
        StartCoroutine(CheckPredrawnHiragana());
    }

    private void ClearPictures()
    {
        drawings[0].SetActive(false);
        drawings[1].SetActive(false);
        drawings[2].SetActive(false);
        drawings[3].SetActive(false);
        drawings[4].SetActive(false);
        StartCoroutine(CheckPredrawnHiragana());
    }

    private void DisableButtons()
    {
        checkButton.interactable = false;
        giveUpButton.interactable = false;
        skipLevelButton.interactable = false;
    }

    private void Action1()
    {
        tutorialSentences = new string[]
        {
            "Welcome To The Tutorial!",
            "Hiragana Sensei is a simple platform for playesr to practice their Hiragana writing skill!",
            "The game goes by answering students with coresponding Hiragana characters.",
        };
        tutorialText.text = tutorialSentences[currentSentenceIndex]; // Set the first sentence
        tutorialTextPanel.SetActive(true);
    }

    private void Action2()
    {
        arrows[0].SetActive(true);
        arrows[1].SetActive(true);
        RaycastHit hit;
        if (Physics.Raycast(player.transform.position, player.transform.forward, out hit))
        {
            if (hit.transform.CompareTag("Front"))
            {
                actionIdx++;
            }
        }
    }

    private void Action3()
    {
        tutorialSentences = new string[]
        {
            "Write the coresponding Hiragana on the center of the board by holding the grip button.",
            "Or simply select the predrawn characters on your right using your trigger button. (Only in tutorial)",
            "Press the duster to clear the board.",
            "Your answer should show on the desk if it is recogniseable.",
        };
        tutorialText.text = tutorialSentences[currentSentenceIndex]; // Set the first sentence

        StartCoroutine(Action3Wait());
    }

    private IEnumerator Action3Wait()
    {
        yield return new WaitForSecondsRealtime(3);

        if (setDelayPanel) { 
            tutorialTextPanel.SetActive(true);
            setDelayPanel = false;
        }
        hiraganaButtonPanel.SetActive(true);
        arrows[0].SetActive(false);
        arrows[1].SetActive(false);
        arrows[2].SetActive(true);
        arrows[3].SetActive(true);
        arrows[4].SetActive(true);
    }

    private void Action4()
    {
        if (!checkDrawing){
            hiraganaChecker.CheckDrawing();
            checkDrawing = true;
        }
        
        if(hiraganaChecker.NormalizeString(hiraganaChecker.recognizedCharacter) == hiraganaChecker.NormalizeString(hiraganaChecker.expectedCharacter))
        {
            actionIdx++;
        }
    }

    private void Action5()
    {
        if (actionIdx == 5) {
            tutorialSentences = new string[]
            {
                "Press the Checkbutton on your desk once you have confirm your answer.",
                "You have 3 attempts before your marks are deducted.",
            };
            tutorialText.text = tutorialSentences[currentSentenceIndex]; // Set the first sentence
            tutorialTextPanel.SetActive(true);
            arrows[2].SetActive(false);
            arrows[3].SetActive(false);
            arrows[5].SetActive(true);
            checkButton.interactable = true;
        }
    }

    private void TutorialbuttonClicked()
    {
        tutorialButtonClicked = true;
    }

    private void Action6()
    {
        tutorialTextPanel.SetActive(false);
        if (tutorialButtonClicked)
        {
            if (hiraganaChecker.NormalizeString(hiraganaChecker.recognizedCharacter) == hiraganaChecker.NormalizeString(hiraganaChecker.expectedCharacter))
            {
                actionIdx++;
            }
        }
    }

    private void Action7()
    {
        tutorialSentences = new string[]
        {
            "The next student will ask the question if you answer correctly or give up.",
            "The game goes on until you can recognize all characters in the current level.",
            "As the game goes on, hints will be reduced.",
            "Press button B to check your progression.",
        };
        tutorialText.text = tutorialSentences[currentSentenceIndex]; // Set the first sentence
        tutorialTextPanel.SetActive(true);
        arrows[4].SetActive(false);
        arrows[5].SetActive(false);
    }

    private void Action8()
    {
        tutorialSentences = new string[]
        {
            "Press button B again to close the panel.",
            "Be aware that your progression will drop if you do not answer correctly.",
            "The tutorial section ends here.",
            "You can always check the inputs in the settings panel.",
            "Press the 'End Tutorial' button to end this secton.",
            "If you wish to continue you can play till the end of this tutorial level",
        };
        tutorialText.text = tutorialSentences[currentSentenceIndex]; // Set the first sentence
        tutorialTextPanel.SetActive(true);      
    }

    private void Action9() {
        if (levelManager.gameLevel > 1)
        {
            EndTutorial();
        }       
    }

    private void ShowNextSentence()
    {
        currentSentenceIndex++;
        
        if (currentSentenceIndex < tutorialSentences.Length)
        {           
            tutorialText.text = tutorialSentences[currentSentenceIndex];
            nextAction = false;
        }

        else if (currentSentenceIndex >= tutorialSentences.Length)
        {
            tutorialTextPanel.SetActive(false);
            nextAction= true;
            currentSentenceIndex = 0;
        }

        else if (tutorialSentences.Length == 0)
        {
            tutorialTextPanel.SetActive(false);
        }       
    }

    private void CheckAction(int actionIdx)
    {
        switch (actionIdx)
        {
            case 1:
                Action1();
                break;

            case 2:
                Action2();
                break;

            case 3:
                Action3();
                break;

            case 4:
                Action4();
                break;            
            
            case 5:
                Action5();
                break;

            case 6:
                Action6();
                break;

            case 7:
                Action7();
                break;

            case 8:
                Action8();
                break;

            case 9:
                Action9();
                break;


            default:
                UnityEngine.Debug.LogError("Invalid action.");
                break;
        }
    }

    private void SkipTutorial()
    {
        EndTutorial();
    }

    private void EndTutorial()
    {
        tutorialTextPanel.SetActive(true);
        tutorialText.text = "Tutoral Section Is Ending, Proceeding To Game";

        Time.timeScale = 1f;
        difficultyData.level = 1;
        difficultyData.difficulty = 1;


        leftHand.SetActive(false);
        rightHand.SetActive(false);
        StartCoroutine(TransitionAfterDelay(1));
    }

    private IEnumerator PressInterval(float pressInterval)
    {
        yield return new WaitForSecondsRealtime(pressInterval);
        canPress = true;
    }    
    
    private IEnumerator CheckPredrawnHiragana()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        hiraganaChecker.CheckDrawing();
    }

    public IEnumerator TransitionAfterDelay(int ScreenIdx)
    {
        screenFader.FadeOut();
        yield return new WaitForSeconds(1.5f); // Wait for the specified delay
        SceneManager.LoadScene(ScreenIdx);
    }
}

