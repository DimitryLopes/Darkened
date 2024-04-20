
[System.Serializable]
public class GameData
{
    public SerializableDictionary<string, UnlockableSavedData> levelSavedData;

    public GameData()
    {
        levelSavedData = new();
    }

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
}
