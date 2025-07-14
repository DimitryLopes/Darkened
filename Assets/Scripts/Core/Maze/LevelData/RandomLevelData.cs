using UnityEngine;

[CreateAssetMenu(fileName = "MazeData", menuName = "Scriptable Objects/Datas/Random Level Data")]
public class RandomLevelData : LevelData
{
    /// <summary>
    /// Will return the minimum torch count based on the difficulty data and size of the maze.
    /// </summary>
    public int MinTorchCount => Mathf.CeilToInt(difficultyData.MinimumTorchRatio * Size);
    /// <summary>
    /// Will return the maximum torch count based on the difficulty data and size of the maze.
    /// </summary>
    public int MaxTorchCount => Mathf.CeilToInt(difficultyData.MaximumTorchRatio * Size);

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
}
