using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;

public class Cheat : MonoBehaviour
{
    [Header("Refrence")]
    [SerializeField] private HiraganaChecker hiraganaChecker; // Reference to the HiraganaChecker script
    [SerializeField] private QuestionSpawner questionSpawner; // Reference to QuestionSpawner
    [SerializeField] private ProgressionUI progressionUI; // Reference to the ProgressionUI script

    [Header("Enable Cheats")]
    [SerializeField] private bool isEnabled = false; // Boolean flag to enable/disable cheats
    [SerializeField] private GameObject cheatTmp;

    [Header("Test Drawings")]
    [SerializeField] private List<GameObject> testDrawings; // List of GameObjects to be activated or deactivated

    [Header("Input Buttons")]
    [SerializeField] public InputActionProperty buttonA; // InputAction for Select
    [SerializeField] public InputActionProperty buttonB; // InputAction for Hiragana Panel
    [SerializeField] private InputActionProperty buttonX; // InputAction for Add Score
    [SerializeField] private InputActionProperty buttonY; // InputAction for next Hiragana
    [SerializeField] private InputActionProperty leftTrigger; // InputAction for Add Score
    [SerializeField] private InputActionProperty rightTrigger; // InputAction for next Hiragana

    private int currentIndex = 0; // The current index to track which object is active

    private void Update()
    {       
        // If all four buttons are pressed simultaneously, toggle the enabled state
        if (leftTrigger.action.triggered && rightTrigger.action.triggered)
        {
            isEnabled = !isEnabled;

            if (isEnabled) {
                cheatTmp.SetActive(true);
            }
            else if (!isEnabled)
            {
                cheatTmp.SetActive(false);
            }
        }


        // Only process input if the functionality is enabled
        if (isEnabled)
        {
            if (buttonX.action.triggered)
            {
                AddScore();
            }

            // Check for input to increase or decrease the index
            if (buttonY.action.triggered)
            {
                IncreaseIndex();
            }

            // Activate and deactivate game objects based on the current index
            UpdateGameObjectVisibility();
        }
    }

    private void OnEnable()
    {
        buttonA.action.Enable();
        buttonB.action.Enable();
        buttonX.action.Enable();
        buttonY.action.Enable();
        leftTrigger.action.Enable();
        rightTrigger.action.Enable();
    }
    private void OnDisable()
    {
        buttonA.action.Disable();
        buttonB.action.Disable();
        buttonX.action.Disable();
        buttonY.action.Disable();
        leftTrigger.action.Disable();
        rightTrigger.action.Disable();
    }

    private void IncreaseIndex()
    {
        // Increase the index and wrap it around if it exceeds the bounds of the list
        currentIndex = (currentIndex + 1) % testDrawings.Count;
        StartCoroutine(CheckPredrawnHiragana());
    }

    private void DecreaseIndex()
    {
        // Decrease the index and wrap it around if it goes below zero
        currentIndex = (currentIndex - 1 + testDrawings.Count) % testDrawings.Count;
    }

    private void UpdateGameObjectVisibility()
    {
        // Loop through all the game objects and enable/disable them based on the current index
        for (int i = 0; i < testDrawings.Count; i++)
        {
            if (i == currentIndex)
            {
                testDrawings[i].SetActive(true); // Enable the current object
            }
            else
            {
                testDrawings[i].SetActive(false); // Disable other objects
            }
        }
    }

    private void AddScore()
    {
        hiraganaChecker.recognizedCharacter = hiraganaChecker.NormalizeString(hiraganaChecker.expectedCharacter);
        hiraganaChecker.CheckPlayerInput();
    }

    private IEnumerator CheckPredrawnHiragana()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        hiraganaChecker.CheckDrawing();
    }
}
