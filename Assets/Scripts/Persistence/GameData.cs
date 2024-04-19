using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public List<UnlockableSavedData> levelSavedData;

    public GameData()
    {
        levelSavedData = new();
    }

    public UnlockableSavedData GetLevelSavedData(string key)
    {
        foreach(UnlockableSavedData unlockableSavedData in levelSavedData)
        {
            if (unlockableSavedData.SavedDataKey != key) continue;

            return unlockableSavedData;
        }

        return null;
    }

    public void SetLevelSavedData(string key, UnlockableSavedData data)
    {
        for(int i = 0; i < levelSavedData.Count; i++)
        {
            if (levelSavedData[i].SavedDataKey != key) continue;

            levelSavedData[i] = data;
            return;
        }

        levelSavedData.Add(data);
    }
}
