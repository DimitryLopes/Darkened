using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialDatabase", menuName = "Scriptable Objects/Data Bases/Tutorial Database")]
public class TutorialDatabase : ScriptableObject
{
    [SerializeField]
    private List<TutorialData> tutorials;

    public Dictionary<TutorialID, TutorialData> TutorialDictionary = new Dictionary<TutorialID, TutorialData>();
    
    public void SetUp()
    {
        foreach (TutorialData tutorial in tutorials)
        {
            TutorialDictionary.Add(tutorial.TutorialID, tutorial);
        }
    }

    public List<TutorialData> GetAllTutorials()
    {
        return tutorials;
    }

    public TutorialData GetTutorial(TutorialID id)
    {
        return TutorialDictionary[id];
    }
}

public enum TutorialID
{
    Move,
    Sprint,
    Interact,
    UseItem,
}
