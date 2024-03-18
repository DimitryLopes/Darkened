using System;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager
{
    private readonly UIScreenDataBase screenDataBase;
    private Dictionary<Type, IScreen> uiScreens = new Dictionary<Type, IScreen>();

    public ScreenManager(UIScreenDataBase screenDataBase)
    {
        this.screenDataBase = screenDataBase;
        SetUpScreenDictionary();
    }

    private void SetUpScreenDictionary()
    {
        List<GameObject> screenPrefabs = screenDataBase.GetScreens();

        foreach (GameObject screenPrefab in screenPrefabs)
        {
            IScreen screen = screenPrefab.GetComponent<IScreen>();
            if (screen != null)
            {
                uiScreens.Add(screen.GetType(), screen);
            }
        }
    }

    public T GetScreen<T>() where T : IScreen
    {
        Type type = typeof(T);
        if (uiScreens.ContainsKey(type))
        {
            return (T)uiScreens[type];
        }
        return default(T);
    }
}