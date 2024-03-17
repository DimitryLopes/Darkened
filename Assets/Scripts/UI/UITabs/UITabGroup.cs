using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class UITabGroup<T> : MonoBehaviour where T : Enum
{
    [SerializeField]
    private List<UITabInfo> uiTabsInfo;

    protected Dictionary<T, UITab> Tabs = new Dictionary<T, UITab>();

    [SerializeField]
    public T defaultTab;

    protected UITab currentTab;

    void Start()
    {
        foreach(UITabInfo info in uiTabsInfo)
        {
            Tabs.Add(info.TabType, info.Tab);
        }

        foreach(T tabType in Enum.GetValues(typeof(T)))
        {
            Tabs[tabType].Deactivate();
        }

        if (defaultTab != null)
        {
            ShowTab(defaultTab);
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
        }
        else
        {
            Debug.LogError($"Tab {tabType} was not in Tabs dictionary");
        }
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

