
[System.Serializable]
public class GameData
{
    public SerializableDictionary<string, UnlockableSavedData> levelSavedData;
    public SerializableDictionary<string, CompletableSavedData> tutorialSavedData;
    public GameData()
    {
        levelSavedData = new();
        tutorialSavedData = new();
    }
    #region Level Data
    public UnlockableSavedData GetLevelSavedData(string key)
    {
        UnlockableSavedData unlockableSavedData;
        levelSavedData.TryGetValue(key, out unlockableSavedData);
        return unlockableSavedData;
    }


    public void SetLevelSavedData(string key, UnlockableSavedData data)
    {
        if (levelSavedData.ContainsKey(key))
        {
            levelSavedData.Remove(key);
        }

        levelSavedData.Add(key, data);
    }
    #endregion

    #region Tutorial Data
    public CompletableSavedData GetTutorialSavedData(string key)
    {
        CompletableSavedData completableSavedData;
        tutorialSavedData.TryGetValue(key, out completableSavedData);
        return completableSavedData;
    }

    public void SetTutorialSavedData(string key, CompletableSavedData data)
    {
        if (tutorialSavedData.ContainsKey(key))
        {
            tutorialSavedData.Remove(key);
        }
        tutorialSavedData.Add(key, data);
    }
    #endregion
}
