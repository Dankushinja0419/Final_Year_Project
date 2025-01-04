using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartMenu : MonoBehaviour
{
    [SerializeField] public ScreenFader screenFader;

    [Header("UI Pages")]
    public GameObject mainMenu;
    public GameObject difficulty;
    public GameObject options;
    public GameObject about;
    public GameObject shop;

    [Header("Main Menu Buttons")]
    public Button startButton;
    public Button optionButton;
    public Button aboutButton;
    public Button shopButton;
    public Button quitButton;
    public List<Button> returnButtons;

    [Header("Difficulty Buttons")]
    public Button beginButton;
    public Button tutorialButton;
    public Button level1Button;
    public Button level2Button;
    public Button level3Button;
    public Button difficulty1Button;
    public Button difficulty2Button;
    public Button difficulty3Button;
    public Button extraLvButton;
    public Button hiddenLvButton;
    public GameObject levelPanel;
    public GameObject difficultyPanel;
    public GameObject extraLvButtonObject;
    public GameObject hiddenLvButtonObject;
    private int openExtraLv;
    private int openHiddenLv;

    [Header("Disable Objects")]
    public GameObject leftHand;
    public GameObject rightHand;

    [Header("Set Difficulty")]
    [SerializeField] private GameData difficultyData;

    private void Awake()
    {
        difficultyData.Load();
    }

    // Start is called before the first frame update
    void Start()
    {
        difficultyData.Save();
        EnableMainMenu();

        //Hook events
        startButton.onClick.AddListener(SelectDifficulty);
        beginButton.onClick.AddListener(StartGame);
        optionButton.onClick.AddListener(EnableOption);
        aboutButton.onClick.AddListener(EnableAbout);
        shopButton.onClick.AddListener(EnableShop);
        quitButton.onClick.AddListener(QuitGame);

        tutorialButton.onClick.AddListener(TutorialLevel);
        level1Button.onClick.AddListener(Level1);
        level2Button.onClick.AddListener(Level2);
        level3Button.onClick.AddListener(Level3);
        difficulty1Button.onClick.AddListener(Difficulty1);
        difficulty2Button.onClick.AddListener(Difficulty2);
        difficulty3Button.onClick.AddListener(Difficulty3);

        extraLvButton.onClick.AddListener(ExtraLv);
        hiddenLvButton.onClick.AddListener(HiddenLv);

        foreach (var item in returnButtons)
        {
            item.onClick.AddListener(EnableMainMenu);
        }

        EnableMainMenu();
    }

    public void QuitGame()
    {
        difficultyData.Save();
        Application.Quit();
    }

    public void StartGame()
    {
        //screenFader.FadeIn();
        if (difficultyData.difficulty <= 0)
        {
            difficultyData.difficulty = 1;
        }

        if (difficultyData.level <= 0)
        {
            difficultyData.level = 1;
        }

        leftHand.SetActive(false);   
        rightHand.SetActive(false);
        StartCoroutine(TransitionAfterDelay(1));
    }

    public void HideAll()
    {
        mainMenu.SetActive(false);
        difficulty.SetActive(false);
        options.SetActive(false);
        about.SetActive(false);
        shop.SetActive(false);
    }

    public void EnableMainMenu()
    {
        mainMenu.SetActive(true);
        difficulty.SetActive(false);
        options.SetActive(false);
        about.SetActive(false);
        shop.SetActive(false);

        difficultyData.level = 0;
        difficultyData.difficulty = 0;
        difficultyData.maxLevel = 3;
        level1Button.interactable = true;
        level2Button.interactable = true;
        level3Button.interactable = true;
        difficulty1Button.interactable = true;
        difficulty2Button.interactable = true;
        difficulty3Button.interactable = true;
        openExtraLv = 0;
        openHiddenLv = 0;

        difficultyData.Save();
        difficultyData.Load();
    }

    public void SelectDifficulty()
    {
        mainMenu.SetActive(false);
        difficulty.SetActive(true);
        options.SetActive(false);
        about.SetActive(false);
        shop.SetActive(false);

        levelPanel.SetActive(true);
        difficultyPanel.SetActive(true);
        extraLvButtonObject.SetActive(false);
        hiddenLvButtonObject.SetActive(false);
    }

    public void EnableOption()
    {
        mainMenu.SetActive(false);
        difficulty.SetActive(false);
        options.SetActive(true);
        about.SetActive(false);
        shop.SetActive(false);
    }

    public void EnableAbout()
    {
        mainMenu.SetActive(false);
        difficulty.SetActive(false);
        options.SetActive(false);
        about.SetActive(true);
        shop.SetActive(false);
    }

    public void EnableShop()
    {
        mainMenu.SetActive(false);
        difficulty.SetActive(false);
        options.SetActive(false);
        about.SetActive(false);
        shop.SetActive(true);
    }

    private void TutorialLevel()
    {
        difficultyData.difficulty = 0;
        difficultyData.level = 0;
        leftHand.SetActive(false);
        rightHand.SetActive(false);
        StartCoroutine(TransitionAfterDelay(2));
    }

    private void Level1()
    {
        difficultyData.level = 1;
        level1Button.interactable = false;
        level2Button.interactable = true;
        level3Button.interactable = true;
    }

    private void Level2()
    {
        difficultyData.level = 2;
        level1Button.interactable = true;
        level2Button.interactable = false;
        level3Button.interactable = true;
    }

    private void Level3()
    {
        difficultyData.level = 3;
        level1Button.interactable = true;
        level2Button.interactable = true;
        level3Button.interactable = false;

        openExtraLv++;

        if (openExtraLv >= 10)
        {
            levelPanel.SetActive(false);
            difficultyPanel.SetActive(false);
            extraLvButtonObject.SetActive(true);
        }
    }

    private void Difficulty1()
    {
        difficultyData.difficulty = 1;
        difficulty1Button.interactable = false;
        difficulty2Button.interactable = true;
        difficulty3Button.interactable = true;
    }

    private void Difficulty2()
    {
        difficultyData.difficulty = 2;
        difficulty1Button.interactable = true;
        difficulty2Button.interactable = false;
        difficulty3Button.interactable = true;
    }

    private void Difficulty3()
    {
        difficultyData.difficulty = 3;
        difficulty1Button.interactable = true;
        difficulty2Button.interactable = true;
        difficulty3Button.interactable = false;

        openExtraLv++;

        if (openExtraLv >= 10)
        {
            levelPanel.SetActive(false);
            difficultyPanel.SetActive(false);
            extraLvButtonObject.SetActive(true);
        }
    }

    private void ExtraLv()
    {
        difficultyData.level = 1;
        difficultyData.difficulty = 2;
        difficultyData.maxLevel = 10;

        openHiddenLv++;

        if(openHiddenLv >= 10)
        {
            levelPanel.SetActive(false);
            difficultyPanel.SetActive(false);
            extraLvButtonObject.SetActive(false);
            hiddenLvButtonObject.SetActive(true);
        }
    }

    private void HiddenLv()
    {
        difficultyData.level = 11;
        difficultyData.difficulty = 3;
        difficultyData.maxLevel = 11;

        levelPanel.SetActive(false);
        difficultyPanel.SetActive(false);
        extraLvButtonObject.SetActive(false);
    }

    public IEnumerator TransitionAfterDelay(int sceneIdx)
    {
        screenFader.FadeOut();
        yield return new WaitForSeconds(1.5f); // Wait for the specified delay
        SceneManager.LoadScene(sceneIdx);
    }
}
