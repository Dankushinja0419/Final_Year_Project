using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class HiraganaChecker : MonoBehaviour
{
    [Header("Refrence")]
    [SerializeField] private QuestionSpawner questionSpawner; // Reference to QuestionSpawner
    [SerializeField] private ProgressionUI progressionUI; // Reference to the ProgressionUI script
    [SerializeField] private Whiteboard whiteboard; // Reference to QuestionSpawner
    [SerializeField] private LevelManager levelManager; // Reference to LevelManager Script
    [SerializeField] private TesseractDriver _tesseractDriver;

    [Header("Buttons")]
    public Button checkButton;
    public Button giveUpButton;
    public Button nextLvButton;
    [SerializeField] private float cooldownTime = 3f; // Cooldown duration of buttons
    private bool checkCd = false;
    public bool giveUpCd = false;
    public bool nextLvCd = false;

    [Header("Display")]
    [SerializeField] public TextMeshProUGUI read; // Text UI to show romaji
    [SerializeField] public TextMeshProUGUI resultDisplay; // Text UI to show result    
    [SerializeField] public TextMeshProUGUI scoreDisplay; // Text UI to show score
    [SerializeField] private RenderTexture playerDrawing; // Texture with player's drawn character

    [Header("Sound Effect")]
    public string correctAudioName;
    public string wrongAudioName;
    public string giveUpAudioName;

    [Header("Timer")]
    [SerializeField] public GameObject timer;
    [SerializeField] public TMP_Text timerText; // Drag your TMP Text object here
    public float maxTime; // Time for each question
    public bool isTimerOn;
    public bool isTimerRunning;
    private float currentTime;

    [Header("Others")]
    public string recognizedCharacter;
    public string expectedCharacter;
    public int score = 0;
    public int tries = 0;

    private string currentRomaji;
    public string CurrentRomaji => currentRomaji; 
    private bool isTesseractReady = false;

    public Dictionary<string, int> correctAttempts = new Dictionary<string, int>(); // Correct attempts
    public Dictionary<string, int> incorrectAttempts = new Dictionary<string, int>(); // Incorrect attempts
    public Dictionary<string, int> giveUpAttempts = new Dictionary<string, int>(); // Incorrect attempts

    public readonly Dictionary<string, string> romajiToHiragana = new Dictionary<string, string>
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
        currentTime = maxTime;
    }

    private void Start()
    {
        // Setup TesseractDriver
        _tesseractDriver = new TesseractDriver();
        _tesseractDriver.Setup(OnTesseractSetupComplete);

        checkButton.onClick.AddListener(CheckAnswer);
        giveUpButton.onClick.AddListener(questionSpawner.GiveUp);
        nextLvButton.onClick.AddListener(NextLevel);

        read.text = "";
        resultDisplay.text = "";
        scoreDisplay.text = score.ToString();
        whiteboard.ClearBoard();

        timerText.text = Mathf.Ceil(currentTime).ToString();
        isTimerRunning = true;
    }

    private void Update()
    {
        if (isTimerOn)
        {
            if (isTimerRunning)
            {
                currentTime -= Time.deltaTime;

                // Update the timer display
                timerText.text = Mathf.Ceil(currentTime).ToString();

                if (currentTime <= 0)
                {
                    isTimerRunning = false;
                    questionSpawner.GiveUp();
                }
            }
        }
    }

    public void OnTesseractSetupComplete()
    {
        isTesseractReady = true;
    }

    public void SetRandomRomaji()
    {
        // Select a random romaji key from the dictionary and set it as the current romaji
        List<string> keys = new List<string>(romajiToHiragana.Keys);
        currentRomaji = keys[Random.Range(0, keys.Count)];

        resultDisplay.text = ""; // Clear previous result        
    }

    public void CheckDrawing()
    {
        if (playerDrawing == null) return;

        Texture2D texture = ConvertRenderTextureToTexture2D(playerDrawing);

        // Recognize the drawn character
        recognizedCharacter = _tesseractDriver.Recognize(texture);
        expectedCharacter = romajiToHiragana[currentRomaji];

        read.text = recognizedCharacter;    
    }

    private void CheckAnswer()
    {
        CheckDrawing();
        CheckPlayerInput();
    }

    public void CheckPlayerInput()
    {
        if (!checkCd)
        {
            StartCoroutine(CheckCooldown());
            ResetTimer();
        }

        // Compare and display result
        if (NormalizeString(recognizedCharacter) == NormalizeString(expectedCharacter))
        {
            AudioManager.instance.Play(correctAudioName);
            score += 100;
            tries = 0;
            correctAttempts[currentRomaji]++;
            scoreDisplay.text = score.ToString();
            resultDisplay.text = "Correct!";
            questionSpawner.OnAnswerChecked(true);
            progressionUI.UpdateTile();
            whiteboard.ClearBoard();
        }
        else
        {
            AudioManager.instance.Play(wrongAudioName);
            tries++;
            incorrectAttempts[currentRomaji]++;

            if (tries >= 3)
            {
                tries = 0;

                if (score > 0)
                {
                    score -= 100; // Reduce lesser score if try but wrong

                    if (score < 0)
                    {
                        score = 0;
                    }
                }

                if (progressionUI.progressionCounters[currentRomaji] > 0)
                {
                    progressionUI.progressionCounters[currentRomaji]--; // Decrement counter if more than 3 try
                }

                progressionUI.UpdateTile();
            }

            scoreDisplay.text = score.ToString();
            resultDisplay.text = (3 - tries) + " Attemps Remaining";
            whiteboard.ClearBoard();
        }        
    }

    public string NormalizeString(string input)
    {
        return input?.Trim().Replace("\r", "").Replace("\n", "");
    }

    private Texture2D ConvertRenderTextureToTexture2D(RenderTexture renderTexture)
    {
        // Create a new Texture2D with the same dimensions as the RenderTexture
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

        // Copy the RenderTexture content to the Texture2D
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        // Reset active RenderTexture
        RenderTexture.active = null;

        return texture;
    }

    public void ResetTimer()
    {
        currentTime = maxTime;
        isTimerRunning = true;
    }

    private IEnumerator CheckCooldown()
    {
        checkCd = true;
        checkButton.interactable = false; // Disable the button
        yield return new WaitForSeconds(cooldownTime);
        checkButton.interactable = true; // Re-enable the button
        checkCd = false;
    }

    public IEnumerator GiveUpCooldown()
    {
        giveUpCd = true;
        giveUpButton.interactable = false; // Disable the button
        yield return new WaitForSeconds(cooldownTime);
        giveUpButton.interactable = true; // Re-enable the button
        giveUpCd = false;
    }
    
    public IEnumerator nextLvCooldown()
    {
        nextLvCd = true;
        nextLvButton.interactable = false; // Disable the button
        yield return new WaitForSeconds(cooldownTime);
        nextLvButton.interactable = true; // Re-enable the button
        nextLvCd = false;
    }

    private void NextLevel()
    {
        levelManager.LevelUp();
        questionSpawner.MoveToRandomStudent();
        StartCoroutine(questionSpawner.DisplayQuestionWithDelay());
    }

}

//ori//
