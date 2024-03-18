using System;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager
{
    private readonly UIScreenDataBase screenDataBase;
    private readonly ScreenFactory screenFactory;
    private Dictionary<Type, IScreen> instantiatedScreens = new Dictionary<Type, IScreen>();

    public ScreenManager(UIScreenDataBase screenDataBase, ScreenFactory screenFactory)
    {
        this.screenDataBase = screenDataBase;
        this.screenFactory = screenFactory;
    }

    public T GetScreen<T>() where T : IScreen
    {
        Type type = typeof(T);
        if (screenDataBase.UIScreens.ContainsKey(type))
        {
            T screen;
            if (instantiatedScreens.ContainsKey(type))
            {
                screen = (T)instantiatedScreens[type];
            }
            else
            {
                screen = GetNewScreen<T>(type);
            }
            return screen;
        }
        Debug.Log($"Screen database doesn't contain a screen of type {type}!");
        return default(T);
    }

    private T GetNewScreen<T>(Type type) where T : IScreen
    {
        T screen;
        T newScreen = (T)screenFactory.Create<T>();
        if (newScreen != null)
        {
            instantiatedScreens.Add(type, newScreen);
        }
        screen = newScreen;
        return screen;
    }
}