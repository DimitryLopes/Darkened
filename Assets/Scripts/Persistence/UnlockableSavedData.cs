[System.Serializable]
public class UnlockableSavedData : SavedData
{
    public bool IsUnlocked;

    public UnlockableSavedData(string key, bool isUnlocked = false) : base (key)
    {
        IsUnlocked = isUnlocked;
    }
}
