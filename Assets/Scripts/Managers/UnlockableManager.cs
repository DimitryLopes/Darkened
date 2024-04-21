using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UnlockableManager
{
    private PersistenceManager persistenceManager;
    private Dictionary<UnlockCondition, List<IUnlockable>> UnlockConditions = new();

    public UnlockableManager(UnlockConditionDataBase unlockConditionDataBase, PersistenceManager persistenceManager)
    {
        this.persistenceManager = persistenceManager;
        SetUpUnlockablesDictionary(unlockConditionDataBase);
    }

    private void SetUpUnlockablesDictionary(UnlockConditionDataBase unlockConditionDataBase)
    {
        foreach (UnlockConditionData data in unlockConditionDataBase.UnlockConditions)
        {
            UnlockConditions.Add(data.UnlockCondition, new List<IUnlockable>());
        }

        List<IUnlockable> unlockables = GetAllUnlockables();

        foreach (IUnlockable unlockable in unlockables)
        {
            UnlockConditions[unlockable.UnlockConditionData.UnlockCondition].Add(unlockable);
        }
    }

    private List<IUnlockable> GetAllUnlockables()
    {
        var unlockables = new List<IUnlockable>();

        // Find all ScriptableObjects in the project
        var scriptableObjects = Resources.FindObjectsOfTypeAll<ScriptableObject>();

        // Iterate through each ScriptableObject
        foreach (var scriptableObject in scriptableObjects)
        {
            // Check if the type of the ScriptableObject implements IDataPersistence
            var type = scriptableObject.GetType();
            if (typeof(IUnlockable).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            {
                // Add the ScriptableObject to the list
                unlockables.Add(scriptableObject as IUnlockable);
            }
        }

        return unlockables;
    }

    public void OnConditionMet(UnlockCondition condition)
    {
        foreach(IUnlockable unlockable in UnlockConditions[condition])
        {
            unlockable.Unlock();
        }
        persistenceManager.SaveGame();
    }
}
