using UnityEngine;
using UnityEditor;

public class UIMainMenuTabGroup : UITabGroup<MainMenuTab>
{
}

public enum MainMenuTab
{
    MainMenu,
    Options,
}
