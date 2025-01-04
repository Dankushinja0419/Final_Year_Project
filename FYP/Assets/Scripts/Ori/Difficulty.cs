using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData")]
public class GameData : ScriptableObject
{
    public int level = 1;
    public int difficulty = 1;
    public int maxLevel = 3;

    public int money = 100000;

    public Color drawColor = Color.black;
    public bool redObtain = false;
    public bool greenObtain = false;
    public bool blueObtain = false;

    public int particleIdx = 0;
    public bool particle1Obtain = false;
    public bool particle2Obtain = false;
    public bool particle3Obtain = false;

    private string PrefKey => $"{name}_SaveData";

    public void Save()
    {
        // Create a temporary saveable data object
        SaveableGameData saveableData = new SaveableGameData
        {
            money = this.money,
            drawColor = this.drawColor,
            redObtain = this.redObtain,
            greenObtain = this.greenObtain,
            blueObtain = this.blueObtain,
            particleIdx = this.particleIdx,
            particle1Obtain = this.particle1Obtain,
            particle2Obtain = this.particle2Obtain,
            particle3Obtain = this.particle3Obtain
        };

        // Serialize to JSON and save in PlayerPrefs
        string json = JsonUtility.ToJson(saveableData);
        PlayerPrefs.SetString(PrefKey, json);
        PlayerPrefs.Save();
        Debug.Log("GameData saved.");
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey(PrefKey))
        {
            string json = PlayerPrefs.GetString(PrefKey);
            SaveableGameData saveableData = JsonUtility.FromJson<SaveableGameData>(json);

            // Load the data back into the ScriptableObject
            this.money = saveableData.money;
            this.drawColor = saveableData.drawColor;
            this.redObtain = saveableData.redObtain;
            this.greenObtain = saveableData.greenObtain;
            this.blueObtain = saveableData.blueObtain;
            this.particleIdx = saveableData.particleIdx;
            this.particle1Obtain = saveableData.particle1Obtain;
            this.particle2Obtain = saveableData.particle2Obtain;
            this.particle3Obtain = saveableData.particle3Obtain;

            Debug.Log("GameData loaded.");
        }
        else
        {
            Debug.LogWarning("No saved data found.");
        }
    }

    public void ClearSave()
    {
        if (PlayerPrefs.HasKey(PrefKey))
        {
            PlayerPrefs.DeleteKey(PrefKey);
            Debug.Log("Saved data cleared.");
        }
    }
}