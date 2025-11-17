#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(ItemDataBase))]
public class ItemDataBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ItemDataBase itemDataBase = (ItemDataBase)target;

        if (itemDataBase == null || itemDataBase.ItemDatas == null)
        {
            base.OnInspectorGUI();
            return;
        }

        // Display each item with its probability
        float totalProbability = 0;
        for (int i = 0; i < itemDataBase.DataBase.Count; i++)
        {
            var itemData = itemDataBase.DataBase[i];
            EditorGUILayout.BeginHorizontal();

            // Display item type and probability slider
            EditorGUILayout.LabelField(itemData.Type.ToString(), GUILayout.Width(100));
            float newProbability = EditorGUILayout.Slider(itemData.Probability, 0, 100);

            // If probability is changed, adjust other items
            if (!Mathf.Approximately(newProbability, itemData.Probability))
            {
                AdjustProbabilities(itemDataBase, i, newProbability - itemData.Probability);
                itemData.Probability = newProbability;
            }

            EditorGUILayout.EndHorizontal();
            totalProbability += itemData.Probability;
        }

        // Display total probability
        EditorGUILayout.LabelField($"Total Probability: {totalProbability}%");

        // Save changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(itemDataBase);
        }
    }

    private void AdjustProbabilities(ItemDataBase itemDataBase, int changedIndex, float delta)
    {
        float remainingDelta = delta;
        int count = itemDataBase.DataBase.Count;

        for (int i = 0; i < count; i++)
        {
            if (i == changedIndex) continue;

            var itemData = itemDataBase.DataBase[i];
            float adjustment = Mathf.Clamp(itemData.Probability - remainingDelta, 0, 100) - itemData.Probability;
            itemData.Probability += adjustment;
            remainingDelta -= adjustment;

            if (Mathf.Approximately(remainingDelta, 0)) break;
        }
    }
}
#endif