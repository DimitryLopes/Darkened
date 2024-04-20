[System.Serializable]
public class UnlockableSavedData : SavedData
{
    public bool IsUnlocked;

    public UnlockableSavedData(bool isUnlocked = false)
    {
        IsUnlocked = isUnlocked;
    }
}
