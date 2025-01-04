using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionUI : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private QuestionSpawner questionSpawner; // Reference to the QuestionSpawner script
    [SerializeField] private HiraganaChecker hiraganaChecker; // Reference to the HiraganaChecker script

    [Header("UI References")]
    [SerializeField] private GameObject progressionPanel;
    [SerializeField] public Image[] hiraganaTiles; // Array of manually placed tile images
    [SerializeField] private Color startColor = new Color(1f, 1f, 1f, 0.5f); // Initial light color
    [SerializeField] private Color endColor = new Color(0f, 1f, 0f, 1f); // Final dark color (green, fully visible)

    public Dictionary<string, int> romajiToTileIndex; // Map romaji to tile indices
    public Dictionary<string, int> progressionCounters = new Dictionary<string, int>(); // Correct answer counters

    private void Awake()
    {
        romajiToTileIndex = new Dictionary<string, int>();

        SetTile();
        PairTiles();
        PairProgressionDictionary();
    }

    public void UpdateTile()
    {
        string currentRomaji = hiraganaChecker.CurrentRomaji;  // Get the current romaji from HiraganaChecker
        int correctCount = progressionCounters[currentRomaji]; // Get correct score form QuestionSpawner
        float correctPercentage = (float)correctCount / questionSpawner.maxScore; // Calculate score percentage

        hiraganaTiles[romajiToTileIndex[currentRomaji]].color = Color.Lerp(startColor, endColor, correctPercentage);
        hiraganaTiles[romajiToTileIndex[currentRomaji]].fillAmount = correctPercentage;
    }

    private void SetTile()
    {
        // Initialize all tiles to the starting color
        foreach (var tile in hiraganaTiles)
        {
            tile.color = startColor;

            // Ensure the tile is of type Image
            if (tile is Image image)
            {
                // Set the image type to Filled
                image.type = Image.Type.Filled;

                // Set the initial fill amount to 0
                image.fillAmount = 0f;
            }
        }
    }

    private void PairTiles()
    {
        // Populate the romajiToTileIndex dictionary
        int index = 0;
        foreach (var pair in hiraganaChecker.romajiToHiragana)
        {
            if (index < hiraganaTiles.Length)
            {
                romajiToTileIndex[pair.Key] = index;
                index++;
            }
        }
    }

    public void PairProgressionDictionary()
    {
        // Initialize counters for each romaji-hiragana pair
        foreach (var pair in hiraganaChecker.romajiToHiragana)
        {
            progressionCounters[pair.Key] = 0;
        }
    }

    public void FinalDisplay()
    {
        progressionPanel.SetActive(true);

        foreach (var tile in hiraganaTiles)
        {
            tile.color = startColor;

            if (tile is Image image)
            {
                // Set the image type to Filled Radial360
                image.type = Image.Type.Filled;
                image.fillMethod = Image.FillMethod.Radial360;
                image.fillAmount = 0f;
            }
        }

        // Loop through each romaji in progressionCounters
        foreach (var pair in progressionCounters)
        {
            string currentRomaji = pair.Key;  // Get the current romaji
            int correctCount = pair.Value;    // Get the correct score from progressionCounters
            int incorrectCount = hiraganaChecker.incorrectAttempts[currentRomaji];
            int giveUpCount = hiraganaChecker.giveUpAttempts[currentRomaji];

            int totalAttempts = correctCount + incorrectCount + giveUpCount;
            float correctPercentage = (float)correctCount / totalAttempts;  // Calculate score percentage
            

            if (totalAttempts > 0)
            {
                // Update the tile's color and fill amount for each romaji
                hiraganaTiles[romajiToTileIndex[currentRomaji]].color = Color.Lerp(startColor, endColor, correctPercentage);
                hiraganaTiles[romajiToTileIndex[currentRomaji]].fillAmount = correctPercentage;
            }
            else
            {
                // Optionally reset the tile if no attempts were made
                hiraganaTiles[romajiToTileIndex[currentRomaji]].color = startColor;
                hiraganaTiles[romajiToTileIndex[currentRomaji]].fillAmount = 0;
            }
        }
    }
}
