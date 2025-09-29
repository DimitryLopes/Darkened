using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialData", menuName = "Scriptable Objects/Tutorial/Tutorial Data")]
public class TutorialData : ScriptableObject, IDataPersistence
{
    [SerializeField]
    private string tutorialText;
    [SerializeField]
    private List<string> requiredInput;
    [SerializeField]
    private TutorialID tutorialID;
    [SerializeField]
    private TutorialTriggerData triggerData;

    public string TutorialText => tutorialText;
    public List<string> RequiredInputs => requiredInput;
    public TutorialID TutorialID => tutorialID;
    public TutorialTriggerData TriggerData => triggerData;

    public CompletableSavedData SavedData { get; private set; }
    public bool IsDirty { get; private set; }

    private string persistenceKey;

    public void LoadData(GameData data)
    {
        var savedData = data.GetTutorialSavedData(persistenceKey);
        if (savedData == null) return;

        SavedData = savedData;
    }

    public new void SetDirty()
    {
        IsDirty = true;
    }

    public void ResetDirty()
    {
        IsDirty = false;
    }

    public void SaveData(ref GameData data)
    {
        data.SetTutorialSavedData(persistenceKey, SavedData);
    }

    public void SetPersistenceKey()
    {
        persistenceKey = string.Format(Constants.Save.PERSISTENCE_TUTORIAL_KEY_FORMAT, TutorialID);
        SavedData = new CompletableSavedData();
    }
}
