using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class QuestionSpawner : MonoBehaviour
{
    [Header("Refrence")]
    [SerializeField] private HiraganaChecker hiraganaChecker; // Reference to the HiraganaChecker script
    [SerializeField] private ProgressionUI progressionUI; // Reference to the ProgressionUI script
    [SerializeField] private LevelManager levelManager; // Reference to LevelManager Script
    [SerializeField] private Whiteboard whiteboard; // Reference to LevelManager Script

    [Header("Position Offsets")]
    [SerializeField] private float offsetX = 0f;
    [SerializeField] private float offsetY = 0f;
    [SerializeField] private float offsetZ = 0f;

    [Header("Size")]
    [SerializeField] private float scaleByDistance = 0.05f;

    [Header("Spawn Delay")]
    [SerializeField] private float delay = 2f;

    [Header("Set-Up")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private TextMeshProUGUI romajiText; // Text for displaying the romaji question (Legacy Text UI)
    [SerializeField] private List<TextMeshProUGUI> hiraganaTexts; // List of 5 Text elements for displaying hiragana answers (Legacy Text UI)
    [SerializeField] private List<GameObject> bubbles; // Bubbles behind text
    [SerializeField] private List<Transform> students; // List of 25 student positions (drag and drop each student's Transform here)

    [Header("Sound Effect")]
    public string popUpAudioName;

    [Header("Difficulty (X)")]
    [SerializeField] public int colorThreshold = 2; // Threshold for color hint
    [SerializeField] public int hideThreshold = 3; // Threshold for no hint
    [SerializeField] public int maxScore = 10; // Max score for each character

    private string currentRomaji;
    private Transform currentStudent;
    private Animator currentStudentAnimator;
    private Animation anim;

    private void Awake()
    {
        levelManager.SetDifficulty(levelManager.difficultyLevel);
    } 

    private void Start()
    {
        mainCamera = Camera.main;
        anim = GetComponent<Animation>();

        // Ensure there are exactly 5 hiragana Text objects
        if (hiraganaTexts.Count != 5)
        {
            Debug.LogError("Please assign exactly 5 hiragana Text elements in the inspector.");
            return;
        }
        MoveToRandomStudent();
        StartCoroutine(DisplayQuestionWithDelay());
    }

    private void Update()
    {
        // Loop through each student and check their animator
        foreach (var student in students)
        {
            Animator studentAnimator = student.GetComponent<Animator>();

            if (studentAnimator != null)
            {
                AnimatorStateInfo stateInfo = studentAnimator.GetCurrentAnimatorStateInfo(0);

                // If animation is not in transition and the "asking" animation is finished, set it back to false
                if (!studentAnimator.IsInTransition(0) && stateInfo.IsName("Asking") && stateInfo.normalizedTime >= 1.0f)
                {
                    studentAnimator.SetBool("asking", false);
                }
            }
        }

        // Calculate the distance from the camera to the QuestionSpawner
        float distance = Vector3.Distance(mainCamera.transform.position, transform.position);

        // Scale the QuestionSpawner based on the distance
        float scaleFactor = 1 + distance * scaleByDistance;
        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

        // Rotate the Question Spawner to always look at the camera
        transform.LookAt(mainCamera.transform);
        transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);

        // Make romajiText face the camera
        romajiText.transform.LookAt(mainCamera.transform);
        romajiText.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);

        // Make all hiraganaTexts face the camera
        foreach (var hiraganaText in hiraganaTexts)
        {
            hiraganaText.transform.LookAt(mainCamera.transform);
            hiraganaText.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
        }
    }

    public void MoveToRandomStudent()
    {
        // Select a random student from the list and move the QuestionSpawner to their position + offset
        currentStudent = students[Random.Range(0, students.Count)];
        Vector3 offsetPosition = currentStudent.position + new Vector3(offsetX, offsetY, offsetZ);

        // Move the QuestionSpawner to the new position with offsets
        transform.position = offsetPosition;

        currentStudentAnimator = currentStudent.GetComponent<Animator>();
        if (currentStudentAnimator != null)
        {
            currentStudentAnimator.SetBool("asking", true);  // Set asking to true for the new student
        }

        HideQuestion();
    }

    public IEnumerator DisplayQuestionWithDelay()
    {
        // Wait for some time for animation to play first
        yield return new WaitForSeconds(delay);

        // Display the romaji question and hiragana answers
        hiraganaChecker.SetRandomRomaji();
        DisplayRomajiQuestion();
    }

    private void DisplayRomajiQuestion()
    {
        AudioManager.instance.Play(popUpAudioName);

        // Show romaji text
        romajiText.enabled = true;
        bubbles[0].SetActive(true);
        anim.Play();

        // Get the current romaji from the HiraganaChecker
        currentRomaji = hiraganaChecker.CurrentRomaji;

        // Display the romaji in the UI
        romajiText.text = currentRomaji;

        // Set up hiragana answers, including one correct answer and four random incorrect ones
        List<string> answers = new List<string> { hiraganaChecker.romajiToHiragana[currentRomaji] }; // Correct answer

        // Add random incorrect answers, ensuring no duplicates
        while (answers.Count < 5)
        {
            string randomAnswer = hiraganaChecker.romajiToHiragana[hiraganaChecker.romajiToHiragana.Keys.ElementAt(Random.Range(0, hiraganaChecker.romajiToHiragana.Count))];
            if (!answers.Contains(randomAnswer))
            {
                answers.Add(randomAnswer);
            }
        }

        // Shuffle answers to randomize the order
        answers = answers.OrderBy(x => Random.value).ToList();

        // Display each hiragana answer in the corresponding Text UI
        for (int i = 0; i < hiraganaTexts.Count; i++)
        {
            hiraganaTexts[i].text = answers[i];

            // Change the color of the correct answer based on the counter
            if (answers[i] == hiraganaChecker.romajiToHiragana[currentRomaji])
            {
                if (progressionUI.progressionCounters[currentRomaji] >= hideThreshold)
                {
                    // Hide all hiragana answer texts
                    foreach (var text in hiraganaTexts)
                    {
                        text.text = "";
                    }
                    return;
                }
                else if (progressionUI.progressionCounters[currentRomaji] < colorThreshold)
                {
                    hiraganaTexts[i].color = Color.blue; // Display in blue if below correctThreshold
                }
                else
                {
                    hiraganaTexts[i].color = Color.black; // Display in black if above correctThreshold but below hideThreshold
                }
            }
            else
            {
                hiraganaTexts[i].color = Color.black; // Default color for incorrect answers
            }
        }
        ShowQuestion();
    }

    // This method is called by HiraganaChecker when the answer is checked
    public void OnAnswerChecked(bool isCorrect)
    {
        if (isCorrect)
        {
            if (progressionUI.progressionCounters[currentRomaji] < maxScore)
            {
                progressionUI.progressionCounters[currentRomaji]++; // Increment counter for the correctly guessed character
            }
            levelManager.CheckLevelChanges();
            MoveToRandomStudent(); // Move to the next random student if the answer is correct
            StartCoroutine(DisplayQuestionWithDelay());
        }
    }

    public void GiveUp()
    {
        hiraganaChecker.ResetTimer();
        if (!hiraganaChecker.giveUpCd)
        {
            StartCoroutine(hiraganaChecker.GiveUpCooldown());
        }

        AudioManager.instance.Play(hiraganaChecker.giveUpAudioName);
        hiraganaChecker.tries = 3;
        hiraganaChecker.giveUpAttempts[currentRomaji]++;
        if (hiraganaChecker.score > 0)
        {
            hiraganaChecker.score -= 200; // Reduce more score if give up

            if (hiraganaChecker.score < 0)
            {
                hiraganaChecker.score = 0;
            }
        }

        if (progressionUI.progressionCounters[currentRomaji] > 0) 
        {
            progressionUI.progressionCounters[currentRomaji]--; // Decrement counter if give up
        }

        whiteboard.ClearBoard();
        hiraganaChecker.scoreDisplay.text = hiraganaChecker.score.ToString(); // Update score
        hiraganaChecker.resultDisplay.text = "Lv Too Hard?"; // Display result

        progressionUI.UpdateTile(); // Update progression color

        MoveToRandomStudent(); // Move to the next random student if the answer is correct
        StartCoroutine(DisplayQuestionWithDelay());
    }

    private void ShowQuestion()
    {
        // Show romaji text
        romajiText.enabled = true;

        // Show all hiragana text
        foreach (var hiraganaText in hiraganaTexts)
        {
            hiraganaText.enabled = true;
        }

        foreach (var bubble in bubbles)
        {
            bubble.SetActive(true);
        }
    }

    public void HideQuestion()
    {
        // Hide romaji text
        romajiText.enabled = false;

        // Hide all hiragana text
        foreach (var hiraganaText in hiraganaTexts)
        {
            hiraganaText.enabled = false;
        }

        foreach (var bubble in bubbles)
        {
            bubble.SetActive(false);
        }
    }
}

//ORI//