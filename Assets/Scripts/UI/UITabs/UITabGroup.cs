using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public abstract class UITabGroup<T> : MonoBehaviour where T : Enum
{
    [SerializeField]
    private List<UITabInfo> uiTabsInfo;

    protected Dictionary<T, UITab> Tabs = new Dictionary<T, UITab>();

    [SerializeField]
    private Button nextTabButton;
    [SerializeField]
    private Button previousTabButton;
    [SerializeField]
    public T defaultTab;

    protected UITab currentTab;
    protected T currentTabType;

    void Start()
    {
        foreach (UITabInfo info in uiTabsInfo)
        {
            Tabs.Add(info.TabType, info.Tab);
            info.Tab.ForceDeactivate();
        }

        if (defaultTab != null)
        {
            ShowTab(defaultTab);
        }

        if (nextTabButton != null)
        {
            nextTabButton.onClick.AddListener(ShowNext);
        }
        if (previousTabButton != null)
        {
            previousTabButton.onClick.AddListener(ShowPrevious);
        }
    }

    public void OnTabClick(int tabIndex)
    {
        T tabType = (T)Enum.ToObject(typeof(T), tabIndex);
        ShowTab(tabType);
    }

    public void ShowTab(T tabType)
    {
        if (Tabs.ContainsKey(tabType))
        {
            if(currentTab != null)
            {
                currentTab.Deactivate();
            }
            Tabs[tabType].Activate();
            currentTab = Tabs[tabType];
            currentTabType = tabType;
        }
        else
        {
            Debug.LogError($"Tab {tabType} was not in Tabs dictionary");
        }
    }

    private void ShowNext()
    {
        int currentTabIntValue = (int)(object)currentTabType;
        int enumLenght = Enum.GetValues(typeof(T)).Length;
        int nextIndex = currentTabIntValue + 1;
        if(nextIndex == enumLenght)
        {
            nextIndex = 0;
        }
        T currentEnumValue = (T)(object)nextIndex;
        ShowTab(currentEnumValue);
    }

    private void ShowPrevious()
    {
        int currentTabIntValue = (int)(object)currentTabType;
        int enumLenght = Enum.GetValues(typeof(T)).Length;
        int nextIndex = currentTabIntValue - 1;
        if (nextIndex < 0)
        {
            nextIndex = enumLenght - 1;
        }
        T currentEnumValue = (T)(object)nextIndex;
        ShowTab(currentEnumValue);
    }

    [Serializable]
    public struct UITabInfo
    {
        [SerializeField]
        private T type;
        [SerializeField]
        private UITab tab;

        public T TabType => type;
        public UITab Tab => tab;
    }
}

