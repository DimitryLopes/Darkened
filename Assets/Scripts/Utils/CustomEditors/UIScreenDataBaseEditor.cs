#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIScreenDataBase))]
public class UIScreenDataBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UIScreenDataBase dataBase = (UIScreenDataBase)target;

        if (GUILayout.Button("Update Screen List"))
        {
            dataBase.UpdateScreenList();
            EditorUtility.SetDirty(dataBase);
        }
    }
}
#endif