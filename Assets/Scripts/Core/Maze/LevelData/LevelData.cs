using UnityEngine;

public abstract class LevelData : ScriptableObject , IUISelectable, IDataPersistence, IUnlockable
{
    [SerializeField, Header("Size")]
    protected MazeSizeData sizeData;

    [SerializeField, Header("Objective")]
    protected ObjectiveData objectiveData;

    [SerializeField, Header("Unlock Condition")]
    private UnlockConditionData unlockConditionData;

    [SerializeField, Header("Enemy")]
    protected EnemyType enemyType;

    [SerializeField, Header("Difficulty"), Tooltip("Used for torch radius")]
    protected DifficultyData difficultyData;

    public string Title => $"Level " + ID;
    public SelectableType SelectableType => SelectableType.Level;

    public int ID { get; set; }
    public MazeSizeData SizeData => sizeData;
    public ObjectiveData ObjectiveData => objectiveData;
    public UnlockConditionData UnlockConditionData => unlockConditionData;
    public DifficultyData DifficultyData => difficultyData;
    public EnemyType EnemyType => enemyType; 
    public int Height => SizeData.Height;
    public int Width => SizeData.Width;
    public int Size => SizeData.Width * SizeData.Height;


    #region Save

    public bool IsDirty { get; private set; }
    public UnlockableSavedData SavedData { get; private set; }
    public string PersistenceKey { get; private set; }

    public void Unlock()
    {
        SavedData.IsUnlocked = true;
        SetDirty();
    }

    public new void SetDirty()
    {
        IsDirty = true;
    }

    public void ResetDirty()
    {
        IsDirty = false;
    }

    public void SetPersistenceKey()
    {
        PersistenceKey = string.Format(Constants.Save.PERSISTENCE_LEVEL_KEY_FORMAT, ID);
        SavedData = new UnlockableSavedData();
    }

    public void LoadData(GameData data)
    {
        var savedData = data.GetLevelSavedData(PersistenceKey);
        if (savedData == null) return;

        SavedData = savedData;
    }

    public void SaveData(ref GameData data)
    {
        data.SetLevelSavedData(PersistenceKey, SavedData);
    }
    #endregion
}