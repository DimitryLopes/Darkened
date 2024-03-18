using UnityEditor;

public class UnityMenus
{
#if UNITY_EDITOR
    [MenuItem("Debug/Go To Loading Screen #1")]
    public static void PlayFromMainMenu()
    {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene($"Assets/Scenes/{Constants.Scenes.LOADING_SCENE}.unity");
    }

    [MenuItem("Debug/Go To Main Menu #2")]
    public static void GoToScene1()
    {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene($"Assets/Scenes/{Constants.Scenes.MAIN_MENU_SCENE}.unity");
    }
#endif
}