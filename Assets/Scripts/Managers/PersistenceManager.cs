using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PersistenceManager
{
    private GameData gameData;
    private List<IDataPersistence> persistences;
    private FilePersistenceHandler persistenceHandler;
    private SignalBus signalBus;

    public PersistenceManager(SignalBus signalBus)
    {
        this.signalBus = signalBus;

        persistences = FindAllPersistences();
        persistenceHandler = new();
    }

    public void NewGame()
    {
        gameData = new();
        signalBus.Fire(new OnNewGameStartedSignal());

        foreach(IDataPersistence persistences in persistences)
        {
            persistences.SaveData(ref gameData);
        }
        SaveGame();
    }

    public void LoadGame()
    {
        gameData = persistenceHandler.Load();

        if (gameData == null)
        {
            NewGame();
        }

        foreach (IDataPersistence persistence in persistences)
        {
            persistence.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        foreach (IDataPersistence persistence in persistences)
        {
            if (!persistence.IsDirty) continue;

            persistence.SaveData(ref gameData);
            persistence.ResetDirty();
        }

        persistenceHandler.Save(gameData);
    }

    public List<IDataPersistence> FindAllPersistences()
    {
        var dataPersistenceObjects = new List<IDataPersistence>();

        // Find all ScriptableObjects in the project
        var scriptableObjects = Resources.FindObjectsOfTypeAll<ScriptableObject>();

        // Iterate through each ScriptableObject
        foreach (var scriptableObject in scriptableObjects)
        {
            // Check if the type of the ScriptableObject implements IDataPersistence
            if (scriptableObject is IDataPersistence)
            {
                // Add the ScriptableObject to the list
                dataPersistenceObjects.Add(scriptableObject as IDataPersistence);
            }
        }

        return dataPersistenceObjects;
    }
}

