using UnityEngine;

[CreateAssetMenu(fileName = "MazeData", menuName = "Scriptable Objects/Datas/Maze Data")]
public class LevelData : ScriptableObject, IUISelectable, IDataPersistence, IUnlockable
{
    [SerializeField]
    private string levelName;

    [SerializeField, Header("Size")]
    private MazeSizeData sizeData;

    [SerializeField, Header("Difficulty")]
    private DifficultyData difficultyData;

    [SerializeField, Range(0, 1), Header("Torches")]
    private float torchRatio;

    [SerializeField, Header("Enemy")]
    private EnemyType enemyType;

    [SerializeField, Header("Objective")]
    private ObjectiveData objective;

    [SerializeField, Header("Unlock Condition")]
    private UnlockConditionData unlockConditionData;

    public int Height => sizeData.Height;
    public int Width => sizeData.Width;
    public int Size => sizeData.Width * sizeData.Height;
    public MazeSizeData SizeData => sizeData;
    public DifficultyData DifficultyData => difficultyData;

    public int MinTorchCount => Mathf.CeilToInt(difficultyData.MinimumTorchRatio * Size);
    public int MaxTorchCount => Mathf.CeilToInt(difficultyData.MaximumTorchRatio * Size);
    public float TorchRatio => torchRatio;
    public EnemyType EnemyType => enemyType;
    public ObjectiveData ObjectiveData => objective;
    public string Title => levelName;
    public SelectableType SelectableType => SelectableType.Level;
    public int ID { get; set; }

    public void SetUp(MazeSizeData sizeData, ObjectiveData objective, DifficultyData difficulty, EnemyType enemyType)
    {
        this.sizeData = sizeData;
        this.objective = objective;
        difficultyData = difficulty;
        torchRatio = (difficultyData.MinimumTorchRatio + difficultyData.MaximumTorchRatio) / 2;
        this.enemyType = enemyType;
    }

    public void SetUp(LevelData data, ObjectiveData objective)
    {
        SetUp(data.sizeData, objective, difficultyData, data.enemyType);
    }

    #region Save

    public bool IsDirty { get; private set; }
    public UnlockableSavedData SavedData { get; private set; }
    public string PersistenceKey { get; private set; }

    public UnlockConditionData UnlockConditionData => unlockConditionData;

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
        SavedData = data.GetLevelSavedData(PersistenceKey);
    }

    public void SaveData(ref GameData data)
    {
        data.SetLevelSavedData(PersistenceKey, SavedData);
    }
    #endregion
}
