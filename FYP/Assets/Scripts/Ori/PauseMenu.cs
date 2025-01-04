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
using static UnityEngine.EventSystems.PointerEventData;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] public ScreenFader screenFader; // Reference to ScreenFader Script
    [SerializeField] private LevelManager levelManager; // Reference to LevelManager Script
    [SerializeField] private GameObject settingsButtonPanal; // Pause menu UI
    [SerializeField] private GameObject checkingPanal; // Checking Panel UI
    [SerializeField] private GameObject settingsMenuPanel; // Pause menu UI
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;

    private HandDraw leftHandDrawScript;
    private HandDraw rightHandDrawScript;

    public Button settingsButton;
    public Button resumeButton;
    public Button exitButton;
    public Button controlsButton;

    public GameObject inputDisplay;

    private void Start()
    {
        settingsButton.onClick.AddListener(OpenSettingsMenu);
        resumeButton.onClick.AddListener(CloseSettingsMenu);
        exitButton.onClick.AddListener(Exit);
        controlsButton.onClick.AddListener(DisplayControls);

        leftHandDrawScript = leftHand.GetComponent<HandDraw>();
        rightHandDrawScript = rightHand.GetComponent<HandDraw>();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; // Pause the game by setting time scale to 0
        leftHandDrawScript.enabled = false;
        rightHandDrawScript.enabled = false;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // Resume the game by setting time scale to 1
        leftHandDrawScript.enabled = true;
        rightHandDrawScript.enabled = true;
    }
    public void Exit() 
    {
        leftHand.SetActive(false);
        rightHand.SetActive(false);
        StartCoroutine(TransitionAfterDelay(0));
    }

    private void OpenSettingsMenu()
    {
        settingsButtonPanal.SetActive(false);
        settingsMenuPanel.SetActive(true);
        checkingPanal.SetActive(false);

        leftHandDrawScript.enabled = false;
        rightHandDrawScript.enabled = false;
    }

    private void CloseSettingsMenu()
    {
        settingsButtonPanal.SetActive(true);
        settingsMenuPanel.SetActive(false);
        inputDisplay.SetActive(false);
        checkingPanal.SetActive(true);

        leftHandDrawScript.enabled = true;
        rightHandDrawScript.enabled = true;
    }

    public void DisplayControls()
    {
        inputDisplay.SetActive(!inputDisplay.activeSelf);
    }

    public IEnumerator TransitionAfterDelay(int sceneIdx)
    {
        screenFader.FadeOut();
        yield return new WaitForSeconds(1.5f); // Wait for the specified delay
        SceneManager.LoadScene(sceneIdx);
    }
}
