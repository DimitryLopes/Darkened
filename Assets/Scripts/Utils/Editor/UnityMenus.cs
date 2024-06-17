using UnityEditor;
using System.IO;
using UnityEngine;

public class UnityMenus
{
#if UNITY_EDITOR
    [MenuItem("Debug/Go To Loading Screen #1")]
    public static void GoToLoadingScene()
    {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene($"Assets/Scenes/{Constants.Scenes.LOADING_SCENE}.unity");
    }

    [MenuItem("Debug/Go To Main Menu #2")]
    public static void GoToMainMenu()
    {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene($"Assets/Scenes/{Constants.Scenes.MAIN_MENU_SCENE}.unity");
    }

    [MenuItem("Debug/Go To Game #3")]
    public static void GoToGame()
    {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene($"Assets/Scenes/Levels/{Constants.Scenes.GAME_SCENE}.unity");
    }

    [MenuItem("Debug/Go To Testing #4")]
    public static void GoToTesting()
    {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene($"Assets/Scenes/{Constants.Scenes.TESTING_SCENE}.unity");
    }

    [MenuItem("Persistence/Delete Saved Data")]
    public static void DeleteSavedData()
    {
        string path = Constants.Save.PERSISTENCE_FILE_PATH;
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Deleted file at: " + path);
        }
    }

    [MenuItem("Persistence/Copy Saved Data path to clipboard")]
    public static void CopySavePath()
    {
        string path = Application.persistentDataPath;
        Debug.Log("path: " + path + " Copyed to clipboard");
        GUIUtility.systemCopyBuffer = path;
    }
#endif
}