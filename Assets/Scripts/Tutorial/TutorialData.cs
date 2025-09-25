using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialData : ScriptableObject
{
    [SerializeField]
    private string tutorialText;
    [SerializeField]
    private List<KeyCode> requiredInput;
    [SerializeField]
    private TutorialID tutorialID;

    public string TutorialText => tutorialText;
    public List<KeyCode> RequiredInputs => requiredInput;
    public TutorialID TutorialID => tutorialID;
}