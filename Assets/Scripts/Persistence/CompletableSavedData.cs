[System.Serializable]
public class CompletableSavedData : SavedData
{
    public bool IsCompleted;

    public CompletableSavedData(bool isCompleted = false)
    {
        IsCompleted = isCompleted;
    }
}
