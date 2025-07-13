using UnityEngine;

public abstract class LevelData : ScriptableObject
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

    public MazeSizeData SizeData => sizeData;
    public ObjectiveData ObjectiveData => objectiveData;
    public UnlockConditionData UnlockConditionData => unlockConditionData;
    public DifficultyData DifficultyData => difficultyData;
    public EnemyType EnemyType => enemyType; 
    public int Height => SizeData.Height;
    public int Width => SizeData.Width;
    public int Size => SizeData.Width * SizeData.Height;
}