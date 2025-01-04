using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class TurnChanger : MonoBehaviour
{
    public ActionBasedContinuousTurnProvider continuousTurnProvider;  // Reference to the ContinuousTurnProvider
    public ActionBasedSnapTurnProvider snapTurnProvider;              // Reference to the SnapTurnProvider
    public TurnType currentTurnType;                                  // Enum to store current turn type

    public Button continuousButton;                                   // Button to activate Continuous Turn
    public Button snapButton;                                         // Button to activate Snap Turn

    void Start()
    {
        // Ensure the providers are properly referenced
        continuousTurnProvider = GetComponent<ActionBasedContinuousTurnProvider>();
        snapTurnProvider = GetComponent<ActionBasedSnapTurnProvider>();

        // Apply saved settings or defaults
        ApplyPlayerPref();

        // Add button click listeners
        continuousButton.onClick.AddListener(ActivateContinuousTurn);
        snapButton.onClick.AddListener(ActivateSnapTurn);
    }

    public enum TurnType
    {
        Continuous,
        Snap
    }

    public void ApplyPlayerPref()
    {
        // Load the saved preference or default to Continuous
        if (PlayerPrefs.HasKey("turn"))
        {
            currentTurnType = (TurnType)PlayerPrefs.GetInt("turn");
        }
        else
        {
            currentTurnType = TurnType.Continuous;
        }

        // Apply the settings
        ApplyTurnSettings();
    }

    public void ApplyTurnSettings()
    {
        // Activate the appropriate turn type
        switch (currentTurnType)
        {
            case TurnType.Continuous:
                ActivateContinuousTurn();
                break;
            case TurnType.Snap:
                ActivateSnapTurn();
                break;
        }
    }

    public void ActivateContinuousTurn()
    {
        // Update button interactivity
        continuousButton.interactable = false;
        snapButton.interactable = true;

        // Enable/disable the respective turn providers
        if (continuousTurnProvider != null) continuousTurnProvider.enabled = true;
        if (snapTurnProvider != null) snapTurnProvider.enabled = false;

        // Save preference
        SaveTurnPreference(TurnType.Continuous);
    }

    public void ActivateSnapTurn()
    {
        // Update button interactivity
        continuousButton.interactable = true;
        snapButton.interactable = false;

        // Enable/disable the respective turn providers
        if (snapTurnProvider != null) snapTurnProvider.enabled = true;
        if (continuousTurnProvider != null) continuousTurnProvider.enabled = false;

        // Save preference
        SaveTurnPreference(TurnType.Snap);
    }

    private void SaveTurnPreference(TurnType turnType)
    {
        // Save the current turn type to PlayerPrefs
        currentTurnType = turnType;
        PlayerPrefs.SetInt("turn", (int)turnType);
        PlayerPrefs.Save();
    }
}
