using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using System;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class LevelManager : MonoBehaviour
{
    [Header("Refrence")]
    private TesseractDriver _tesseractDriver;
    [SerializeField] private HiraganaChecker hiraganaChecker; // Reference to the HiraganaChecker script
    [SerializeField] private QuestionSpawner questionSpawner; // Reference to the ProgressionUI script
    [SerializeField] private ProgressionUI progressionUI; // Reference to the ProgressionUI script
    [SerializeField] private PauseMenu pauseMenu; // Reference to the ProgressionUI script
    [SerializeField] private MaterialColor materialColor; // Reference to the MaterialColor script
    [SerializeField] public ScreenFader screenFader; // Reference to ScreenFader script
    [SerializeField] private Whiteboard whiteboard; // Reference to QuestionSpawner

    [Header("Level Up UI")]
    [SerializeField] public GameObject levelUpPanel; // Level Up UI panel
    [SerializeField] public GameObject generalMessagePanel; // General Message UI panel
    [SerializeField] public GameObject[] sideImage; // Level Up UI panel side image
    [SerializeField] public TextMeshProUGUI currentLevelDisplay; // Level: N
    [SerializeField] public InputActionProperty buttonA; // InputAction for next Level
    [SerializeField] public InputActionProperty buttonX; // InputAction for back to main menu

    [Header("Piechart Text")]
    [SerializeField] public TextMeshProUGUI accuracyTittle; 
    [SerializeField] private TextMeshProUGUI totalCorrectTMPDisplay;
    [SerializeField] private TextMeshProUGUI totalIncorrectTMPDisplay;
    [SerializeField] private TextMeshProUGUI totalGiveUpTMPDisplay;
    [SerializeField] private TextMeshProUGUI totalQuestionAnswered;
    [SerializeField] public TextMeshProUGUI generalNotice;

    [Header("Piechart Color")]
    [SerializeField] private Image correctImage;
    [SerializeField] private Image incorrectImage;
    [SerializeField] private Image giveUpImage;

    [Header("Sound Effect")]
    public string levelUpAudioName;

    [Header("Difficulty")]
    [SerializeField] private GameData difficultyData;
    [SerializeField] private GameObject outsideSchool;
    private Vector3 originalOutsideHeight;

    [Header("Hands")]
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;

    [Header("Others")]
    [SerializeField] private GameObject questionSpawnerObject;

    public int gameLevel = 1;
    public int difficultyLevel = 1;
    public int maxLevel = 3;
    private bool isLvUp = false;
    private bool displayFinalResult = false;
    private bool displayFinalProgression = false;
    private bool isEndGame = false;
    private bool allReachedMaxScore;

    private readonly Dictionary<string, string> level1Characters = new Dictionary<string, string>
    {
        { "a", "あ" }, { "i", "い" }, { "u", "う" }, { "e", "え" }, { "o", "お" },
    };

    private readonly Dictionary<string, string> level2Characters = new Dictionary<string, string>
    {
        //{ "a", "あ" }, { "i", "い" }, { "u", "う" }, { "e", "え" }, { "o", "お" },
        { "ka", "か" }, { "ki", "き" }, { "ku", "く" }, { "ke", "け" }, { "ko", "こ" },
    };

    private readonly Dictionary<string, string> level3Characters = new Dictionary<string, string>
    {
        //{ "a", "あ" }, { "i", "い" }, { "u", "う" }, { "e", "え" }, { "o", "お" },
        //{ "ka", "か" }, { "ki", "き" }, { "ku", "く" }, { "ke", "け" }, { "ko", "こ" },
        { "sa", "さ" }, { "shi", "し" }, { "su", "す" }, { "se", "せ" }, { "so", "そ" },
    };

    private readonly Dictionary<string, string> level4Characters = new Dictionary<string, string>
    {
        { "ta", "た" }, { "chi", "ち" }, { "tsu", "つ" }, { "te", "て" }, { "to", "と" },
    };

    private readonly Dictionary<string, string> level5Characters = new Dictionary<string, string>
    {
        { "na", "な" }, { "ni", "に" }, { "nu", "ぬ" }, { "ne", "ね" }, { "no", "の" },
    };

    private readonly Dictionary<string, string> level6Characters = new Dictionary<string, string>
    {
        { "ha", "は" }, { "hi", "ひ" }, { "fu", "ふ" }, { "he", "へ" }, { "ho", "ほ" },
    };

    private readonly Dictionary<string, string> level7Characters = new Dictionary<string, string>
    {
        { "ma", "ま" }, { "mi", "み" }, { "mu", "む" }, { "me", "め" }, { "mo", "も" },
    };

    private readonly Dictionary<string, string> level8Characters = new Dictionary<string, string>
    {
        { "ya", "や" }, { "yu", "ゆ" }, { "yo", "よ" },
    };

    private readonly Dictionary<string, string> level9Characters = new Dictionary<string, string>
    {
        { "ra", "ら" }, { "ri", "り" }, { "ru", "る" }, { "re", "れ" }, { "ro", "ろ" },
    };

    private readonly Dictionary<string, string> level10Characters = new Dictionary<string, string>
    {
        { "wa", "わ" }, { "wo", "を" },
        { "n", "ん" },
    };

    private readonly Dictionary<string, string> level11Characters = new Dictionary<string, string>
    {
        { "a", "あ" }, { "i", "い" }, { "u", "う" }, { "e", "え" }, { "o", "お" },
        { "ka", "か" }, { "ki", "き" }, { "ku", "く" }, { "ke", "け" }, { "ko", "こ" },
        { "sa", "さ" }, { "shi", "し" }, { "su", "す" }, { "se", "せ" }, { "so", "そ" },
        { "ta", "た" }, { "chi", "ち" }, { "tsu", "つ" }, { "te", "て" }, { "to", "と" },
        { "na", "な" }, { "ni", "に" }, { "nu", "ぬ" }, { "ne", "ね" }, { "no", "の" },
        { "ha", "は" }, { "hi", "ひ" }, { "fu", "ふ" }, { "he", "へ" }, { "ho", "ほ" },
        { "ma", "ま" }, { "mi", "み" }, { "mu", "む" }, { "me", "め" }, { "mo", "も" },
        { "ya", "や" }, { "yu", "ゆ" }, { "yo", "よ" },
        { "ra", "ら" }, { "ri", "り" }, { "ru", "る" }, { "re", "れ" }, { "ro", "ろ" },
        { "wa", "わ" }, { "wo", "を" },
        { "n", "ん" },
        // Add more mappings as needed
    };


    private void Awake()
    {
        PairAcuracyDictionary();

        maxLevel = difficultyData.maxLevel;

        gameLevel = difficultyData.level;
        SetLevel(gameLevel);

        difficultyLevel = difficultyData.difficulty;
        SetDifficulty(difficultyLevel);

        correctImage.fillAmount = 0f;
        incorrectImage.fillAmount = 0f;
        giveUpImage.fillAmount = 0f;

        generalNotice.text = "";
    }

    private void Start()
    {
        screenFader.FadeIn();
        originalOutsideHeight = outsideSchool.transform.position;
    }

    private void Update()
    {
        if (isLvUp)
        {
            hiraganaChecker.checkButton.interactable = false;
            hiraganaChecker.giveUpButton.interactable = false;
            hiraganaChecker.nextLvButton.interactable = false;

            if (buttonA.action.triggered)
            {
                hiraganaChecker.checkButton.interactable = true;
                hiraganaChecker.giveUpButton.interactable = true;
                hiraganaChecker.nextLvButton.interactable = true;
                pauseMenu.ResumeGame();
                levelUpPanel.SetActive(false);
                generalMessagePanel.SetActive(false);
                isLvUp = false;
                StartCoroutine(SwitchLvFade());
                //questionSpawner.MoveToRandomStudent();
                //StartCoroutine(questionSpawner.DisplayQuestionWithDelay());
            }
        }
        if (displayFinalResult)
        {
            hiraganaChecker.checkButton.interactable = false;
            hiraganaChecker.giveUpButton.interactable = false;
            hiraganaChecker.nextLvButton.interactable = false;
            questionSpawner.HideQuestion();
            questionSpawnerObject.SetActive(false);

            if (buttonA.action.triggered)
            {
                pauseMenu.ResumeGame();
                accuracyTittle.text = "Overall Accuracy";
                levelUpPanel.SetActive(true);
                sideImage[0].SetActive(false);
                sideImage[1].SetActive(false);
                sideImage[2].SetActive(true);
                CalculateFinalResults();
                generalMessagePanel.SetActive(true);
                generalNotice.text = "Press A to show accuracy of character";
                pauseMenu.PauseGame();
                StartCoroutine(fianlDisplayPause(1f));
            }
        }
        if (displayFinalProgression)
        {
            hiraganaChecker.checkButton.interactable = false;
            hiraganaChecker.giveUpButton.interactable = false;
            hiraganaChecker.nextLvButton.interactable = false;

            if (buttonA.action.triggered)
            {
                pauseMenu.ResumeGame();
                questionSpawner.HideQuestion();
                levelUpPanel.SetActive(false);
                progressionUI.FinalDisplay();
                generalMessagePanel.SetActive(true);
                generalNotice.text = "Press A to return to main menu";
                pauseMenu.PauseGame();
                displayFinalResult = false;
                StartCoroutine(FinaldisplayCD());
            }
        }
        if (isEndGame)
        {
            if (buttonA.action.triggered)
            {
                questionSpawner.HideQuestion();
                levelUpPanel.SetActive(false);
                pauseMenu.ResumeGame();
                leftHand.SetActive(false);
                rightHand.SetActive(false);
                StartCoroutine(TransitionAfterDelay(0));
            }
        }
    }

    private void OnEnable()
    {
        buttonA.action.Enable();
        buttonX.action.Enable();
    }

    private void OnDisaable()
    {
        buttonA.action.Enable();
        buttonX.action.Enable();
    }

    public void CheckLevelChanges()
    {
        allReachedMaxScore = true;

        foreach (var pair in hiraganaChecker.romajiToHiragana)
        {
            // Check if the current progression counter matches maxScore
            if (progressionUI.progressionCounters[pair.Key] < questionSpawner.maxScore)
            {
                allReachedMaxScore = false; // Set to false if any pair hasn't reached maxScore
                break; // Exit early since we found one that doesn't match
            }
        }

        if (allReachedMaxScore)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        if (!hiraganaChecker.nextLvCd)
        {
            StartCoroutine(hiraganaChecker.nextLvCooldown());
        }

        AudioManager.instance.Play(levelUpAudioName);
        difficultyData.money += hiraganaChecker.score / 10;
       
        if (difficultyData.money >= 999999999)
        {
            difficultyData.money = 999999999;
        }
        difficultyData.Save();

        questionSpawner.HideQuestion();
        hiraganaChecker.ResetTimer();
        whiteboard.ClearBoard();

        if (gameLevel < maxLevel)
        {
            CalculateLevelResults();
            levelUpPanel.SetActive(true);
            sideImage[0].SetActive(true);
            sideImage[1].SetActive(false);
            sideImage[2].SetActive(false);
            generalMessagePanel.SetActive(true);
            generalNotice.text = "Press A to next level";
            accuracyTittle.text = "Level " + gameLevel + " Accuracy";
            isLvUp = true;
            pauseMenu.PauseGame();
        }
        else if(gameLevel >= maxLevel)
        {
            CalculateLevelResults();
            levelUpPanel.SetActive(true);
            sideImage[0].SetActive(false);
            sideImage[1].SetActive(true);
            sideImage[2].SetActive(false);
            generalMessagePanel.SetActive(true);
            generalNotice.text = "Press A to show total performance";
            accuracyTittle.text = "Level " + gameLevel + " Accuracy";
            displayFinalResult = true;
            pauseMenu.PauseGame();
        }
    }

    public void SetLevel(int level)
    {
        outsideSchool.transform.position = new Vector3(transform.position.x, originalOutsideHeight.y - level * 2, transform.position.z);
        hiraganaChecker.romajiToHiragana.Clear();
        currentLevelDisplay.text = "Level " + level.ToString();
        materialColor.ChangeMaterialByLevel(level);
        _tesseractDriver = new TesseractDriver();
        switch (level)
        {
            case 0:
                currentLevelDisplay.text = "Tutorial";
                _tesseractDriver.whitelist = "あいうえお";
                //_tesseractDriver.blacklist = "かきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわをん";
                foreach (var pair in level1Characters)
                {
                    hiraganaChecker.romajiToHiragana.Add(pair.Key, pair.Value);
                }
                break;

            case 1:
                _tesseractDriver.whitelist = "あいうえお";
                //_tesseractDriver.blacklist = "かきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわをん";
                foreach (var pair in level1Characters)
                {
                    hiraganaChecker.romajiToHiragana.Add(pair.Key, pair.Value);
                }                   
                break;

            case 2:
                _tesseractDriver.whitelist = "かきくけこ";
                //_tesseractDriver.blacklist = "あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわをん";
                foreach (var pair in level2Characters)
                {
                    hiraganaChecker.romajiToHiragana.Add(pair.Key, pair.Value);
                }                    
                break;

            case 3:
                _tesseractDriver.whitelist = "さしすせそ";
                //_tesseractDriver.blacklist = "あいうえおかきくけこたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわをん";
                foreach (var pair in level3Characters)
                {
                    hiraganaChecker.romajiToHiragana.Add(pair.Key, pair.Value);
                }               
                break;

            case 4:
                _tesseractDriver.whitelist = "たちつてと";
                //_tesseractDriver.blacklist = "あいうえおかきくけこさしすせそなにぬねのはひふへほまみむめもやゆよらりるれろわをん";
                foreach (var pair in level4Characters)
                {
                    hiraganaChecker.romajiToHiragana.Add(pair.Key, pair.Value);
                }
                break;

            case 5:
                _tesseractDriver.whitelist = "なにぬねの";
                //_tesseractDriver.blacklist = "あいうえおかきくけこさしすせそたちつてとはひふへほまみむめもやゆよらりるれろわをん";
                foreach (var pair in level5Characters)
                {
                    hiraganaChecker.romajiToHiragana.Add(pair.Key, pair.Value);
                }
                break;

            case 6:
                _tesseractDriver.whitelist = "はひふへほ";
                //_tesseractDriver.blacklist = "あいうえおかきくけこさしすせそたちつてとなにぬねのまみむめもやゆよらりるれろわをん";
                foreach (var pair in level6Characters)
                {
                    hiraganaChecker.romajiToHiragana.Add(pair.Key, pair.Value);
                }
                break;

            case 7:
                _tesseractDriver.whitelist = "まみむめも";
                //_tesseractDriver.blacklist = "あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほやゆよらりるれろわをん";
                foreach (var pair in level7Characters)
                {
                    hiraganaChecker.romajiToHiragana.Add(pair.Key, pair.Value);
                }
                break;

            case 8:
                _tesseractDriver.whitelist = "やゆよ";
                //_tesseractDriver.blacklist = "あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもらりるれろわをん";
                foreach (var pair in level8Characters)
                {
                    hiraganaChecker.romajiToHiragana.Add(pair.Key, pair.Value);
                }
                break;

            case 9:
                _tesseractDriver.whitelist = "らりるれろ";
                //_tesseractDriver.blacklist = "あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよわをん";
                foreach (var pair in level9Characters)
                {
                    hiraganaChecker.romajiToHiragana.Add(pair.Key, pair.Value);
                }
                break;

            case 10:
                _tesseractDriver.whitelist = "わをん";
                //_tesseractDriver.blacklist = "あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろ";
                foreach (var pair in level10Characters)
                {
                    hiraganaChecker.romajiToHiragana.Add(pair.Key, pair.Value);
                }
                break;

            case 11:
                currentLevelDisplay.text = "Level MAX";
                _tesseractDriver.whitelist = "あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわをん";
                foreach (var pair in level11Characters)
                {
                    hiraganaChecker.romajiToHiragana.Add(pair.Key, pair.Value);
                }
                break;

            default:
                UnityEngine.Debug.LogError("Invalid level selected.");
                break;
        }
        _tesseractDriver.Setup(hiraganaChecker.OnTesseractSetupComplete);
    }

    public void SetDifficulty(int difficulty)
    {
        switch (difficulty)
        {
            case 0: // Tutorial
                hiraganaChecker.timer.SetActive(false);
                hiraganaChecker.isTimerOn = false;
                hiraganaChecker.isTimerRunning = false;
                questionSpawner.colorThreshold = 1;
                questionSpawner.hideThreshold = 2;
                questionSpawner.maxScore = 3;
                break;

            case 1: // Color + no color
                hiraganaChecker.timer.SetActive(true);
                hiraganaChecker.isTimerOn = true;
                hiraganaChecker.isTimerRunning = true;
                hiraganaChecker.maxTime = 100f;
                questionSpawner.colorThreshold = 3;
                questionSpawner.hideThreshold = 5;
                questionSpawner.maxScore = 5;
                break;

            case 2: // Color + no color + no hint
                hiraganaChecker.timer.SetActive(true);
                hiraganaChecker.isTimerOn = true;
                hiraganaChecker.isTimerRunning = true;
                hiraganaChecker.maxTime = 60f;
                questionSpawner.colorThreshold = 1;
                questionSpawner.hideThreshold = 8;
                questionSpawner.maxScore = 10;
                break;

            case 3: // No hint
                hiraganaChecker.timer.SetActive(true);
                hiraganaChecker.isTimerOn = true;
                hiraganaChecker.isTimerRunning = true;
                hiraganaChecker.maxTime = 30f;
                questionSpawner.colorThreshold = 0;
                questionSpawner.hideThreshold = 0;
                questionSpawner.maxScore = 20;
                break;

            default:
                UnityEngine.Debug.LogError("Invalid difficulty selected.");
                break;
        }
    }

    private void PairAcuracyDictionary()
    {
        foreach (var pair in hiraganaChecker.romajiToHiragana)
        {
            hiraganaChecker.correctAttempts[pair.Key] = 0;
        }

        foreach (var pair in hiraganaChecker.romajiToHiragana)
        {
            hiraganaChecker.incorrectAttempts[pair.Key] = 0;
        }

        foreach (var pair in hiraganaChecker.romajiToHiragana)
        {
            hiraganaChecker.giveUpAttempts[pair.Key] = 0;
        }
    }

    public void CalculateFinalResults()
    {
        // Sum of results of all hiragana
        int totalCorrect = hiraganaChecker.correctAttempts.Values.Sum();
        int totalIncorrect = hiraganaChecker.incorrectAttempts.Values.Sum();
        int totalGiveUp = hiraganaChecker.giveUpAttempts.Values.Sum();

        // Total attempts
        float totalAttempts = totalCorrect + totalIncorrect + totalGiveUp;

        // Calculate percentages
        float correctPercent = totalCorrect / totalAttempts;
        float incorrectPercent = totalIncorrect / totalAttempts;
        float giveUpPercent = totalGiveUp / totalAttempts;

        // Pie Chart
        correctImage.fillAmount = correctPercent; // Green segment
        incorrectImage.fillAmount = correctPercent + incorrectPercent; // Red segment
        giveUpImage.fillAmount = 1f; // Yellow segment (fills the remaining space)

        totalCorrectTMPDisplay.text = correctPercent.ToString("F2") + "%";
        totalIncorrectTMPDisplay.text = incorrectPercent.ToString("F2") + "%";
        totalGiveUpTMPDisplay.text = giveUpPercent.ToString("F2") + "%";
        totalQuestionAnswered.text = totalAttempts.ToString();

        // Avoid division by zero
        if (totalAttempts == 0)
        {
            totalCorrectTMPDisplay.text = "0%";
            totalIncorrectTMPDisplay.text = "0%";
            totalGiveUpTMPDisplay.text = "0%";
            totalQuestionAnswered.text = "0";
            return;
        }
    }

    public void CalculateLevelResults()
    {
        // Initialize counters
        int totalCorrect = 0;
        int totalIncorrect = 0;
        int totalGiveUp = 0;

        // Sum the results for only the keys present in romajiToHiragana
        foreach (var romaji in hiraganaChecker.romajiToHiragana.Keys)
        {
            totalCorrect += hiraganaChecker.correctAttempts[romaji];
            totalIncorrect += hiraganaChecker.incorrectAttempts[romaji];
            totalGiveUp += hiraganaChecker.giveUpAttempts[romaji];
        }

        // Total attempts
        float totalAttempts = totalCorrect + totalIncorrect + totalGiveUp;

        // Calculate percentages
        float correctPercent = totalCorrect / totalAttempts;
        float incorrectPercent = totalIncorrect / totalAttempts;
        float giveUpPercent = totalGiveUp / totalAttempts;

        // Update the pie chart
        correctImage.fillAmount = correctPercent; // Green segment
        incorrectImage.fillAmount = correctPercent + incorrectPercent; // Red segment
        giveUpImage.fillAmount = 1f; // Yellow segment (fills the remaining space)

        // Display results with two decimal places
        totalCorrectTMPDisplay.text = (correctPercent * 100).ToString("F2") + "%";
        totalIncorrectTMPDisplay.text = (incorrectPercent * 100).ToString("F2") + "%";
        totalGiveUpTMPDisplay.text = (giveUpPercent * 100).ToString("F2") + "%";
        totalQuestionAnswered.text = totalAttempts.ToString();

        // Avoid division by zero
        if (totalAttempts == 0)
        {
            totalCorrectTMPDisplay.text = "0%";
            totalIncorrectTMPDisplay.text = "0%";
            totalGiveUpTMPDisplay.text = "0%";
            totalQuestionAnswered.text = "0";
            return;
        }
    }

    public IEnumerator fianlDisplayPause(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        displayFinalResult = false;
        displayFinalProgression = true;
    }

    public IEnumerator SwitchLvFade()
    {
        screenFader.FadeOut();
        yield return new WaitForSecondsRealtime(1.5f); // Wait for the specified delay
        gameLevel++;
        SetLevel(gameLevel);
        screenFader.FadeIn();
    }    
    
    public IEnumerator TransitionAfterDelay(int screenIdx)
    {
        screenFader.FadeOut();
        yield return new WaitForSecondsRealtime(1.5f); // Wait for the specified delay
        SceneManager.LoadScene(0);
    }

    public IEnumerator FinaldisplayCD()
    {
        yield return new WaitForSecondsRealtime(1.5f); // Wait for the specified delay
        isEndGame = true;
    }
}

