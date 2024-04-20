using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FilePersistenceHandler
{
    public GameData Load()
    {
        string path = Constants.Save.PERSISTENCE_FILE_PATH;
        GameData loadedData = null;
        if (File.Exists(path))
        {
            try
            {
                string dataToLoad = string.Empty;

                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch(Exception e)
            {
                Debug.LogError($"Couldn't load GameData: \n Path: { path} \n Exception: {e}");
            }
        }
        return loadedData;
    }

    public void Save(GameData data)
    {
        string path = Constants.Save.PERSISTENCE_FILE_PATH;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            string dataToStore = JsonUtility.ToJson(data, true);

            using (FileStream stream =  new FileStream(path, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Couldn't save GameData: \n Path: { path} \n Exception: {e}");
        }
    }
}
