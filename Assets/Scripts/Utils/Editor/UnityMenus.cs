using UnityEditor;

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

    [MenuItem("Debug/Go To Testing #3")]
    public static void GoToTesting()
    {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene($"Assets/Scenes/{Constants.Scenes.TESTING_SCENE}.unity");
    }
#endif
}