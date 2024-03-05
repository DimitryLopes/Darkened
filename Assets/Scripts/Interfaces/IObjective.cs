using System.Collections.Generic;

public interface IObjective
{
    float Progress { get; }
    bool IsCompleted { get; }
    string Description { get; }
    void StartObjective();
    void Clear();
    List<ItemType> GetRequiredItems();
    void CompleteMission();
    void UpdateProgress(float amount);
    void CompleteObjective();
}
