using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIScreenDataBase", menuName = "Scriptable Objects/UI/UIScreenDataBase")]
public class UIScreenDataBase : ScriptableObject
{
    public const string SCREEN_PREFAB_PATH = "Assets/Prefabs/UI/Screens";

    [ReadOnly]
    public List<GameObject> uiScreens;

    public void UpdateScreenList()
    {
        uiScreens.Clear();
        uiScreens = new List<GameObject>();

        var prefabGuids = UnityEditor.AssetDatabase.FindAssets("t:prefab", new[] { SCREEN_PREFAB_PATH });

        foreach (var prefabGuid in prefabGuids)
        {
            string prefabPath = UnityEditor.AssetDatabase.GUIDToAssetPath(prefabGuid);
            var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            var screen = prefab.GetComponent<IScreen>();

            if (screen != null)
            {
                uiScreens.Add(prefab);
            }
        }
    }

    public List<GameObject> GetScreens()
    {
        return uiScreens;
    }
}