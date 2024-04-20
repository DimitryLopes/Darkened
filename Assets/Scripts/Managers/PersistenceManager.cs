using System.Collections.Generic;
using UnityEngine;

public class PersistenceManager
{
    private GameData gameData;
    private List<IDataPersistence> persistences;
    private FilePersistenceHandler persistenceHandler;

    public PersistenceManager()
    {
        persistences = FindAllPersistences();
        persistenceHandler = new();
    }

    public void NewGame()
    {
        gameData = new();
        foreach(IDataPersistence persistences in persistences)
        {
            persistences.SaveData(ref gameData);
        }
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
            var type = scriptableObject.GetType();
            if (typeof(IDataPersistence).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            {
                // Add the ScriptableObject to the list
                dataPersistenceObjects.Add(scriptableObject as IDataPersistence);
            }
        }

        return dataPersistenceObjects;
    }
}

