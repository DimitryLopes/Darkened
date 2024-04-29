using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumValuesAttribute))]
public class EnumValuesAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginChangeCheck();

        int newValue = EditorGUI.MaskField(position, label, property.intValue, property.enumDisplayNames);

        if (EditorGUI.EndChangeCheck())
        {
            property.intValue = newValue;
        }
    }
}

public class EnumValuesAttribute : PropertyAttribute
{
    public EnumValuesAttribute() { }
}