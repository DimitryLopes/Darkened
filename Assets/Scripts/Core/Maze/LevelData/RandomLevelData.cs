using UnityEngine;

[CreateAssetMenu(fileName = "MazeData", menuName = "Scriptable Objects/Datas/Random Level Data")]
public class RandomLevelData : LevelData, IUISelectable, IDataPersistence, IUnlockable
{
    [SerializeField]
    private string levelName;


    [SerializeField, Header("Unlock Condition")]
    private UnlockConditionData unlockConditionData;


    /// <summary>
    /// Will return the minimum torch count based on the difficulty data and size of the maze.
    /// </summary>
    public int MinTorchCount => Mathf.CeilToInt(difficultyData.MinimumTorchRatio * Size);
    /// <summary>
    /// Will return the maximum torch count based on the difficulty data and size of the maze.
    /// </summary>
    public int MaxTorchCount => Mathf.CeilToInt(difficultyData.MaximumTorchRatio * Size);
    public string Title => levelName;
    public SelectableType SelectableType => SelectableType.Level;
    public int ID { get; set; }

    public void SetUp(MazeSizeData sizeData, ObjectiveData objectiveData, DifficultyData difficulty, EnemyType enemyType)
    {
        this.sizeData = sizeData;
        this.objectiveData = objectiveData;
        difficultyData = difficulty;
        this.enemyType = enemyType;
    }

    public void SetUp(RandomLevelData data, ObjectiveData objective)
    {
        SetUp(data.sizeData, objective, difficultyData, data.enemyType);
    }

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
        SavedData = data.GetLevelSavedData(PersistenceKey);
    }

    public void SaveData(ref GameData data)
    {
        data.SetLevelSavedData(PersistenceKey, SavedData);
    }
    #endregion
}
