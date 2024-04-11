using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ScreenManager
{
    private readonly UIScreenDataBase screenDataBase;
    private readonly ScreenFactory screenFactory;
    private Dictionary<Type, IScreen> instantiatedScreens = new Dictionary<Type, IScreen>();
    private IScreen currentScreen;

    public bool IsShowingScreen => currentScreen != null;

    public ScreenManager(UIScreenDataBase screenDataBase, ScreenFactory screenFactory, SignalBus signalBus)
    {
        this.screenDataBase = screenDataBase;
        this.screenFactory = screenFactory;

        signalBus.Subscribe<OnScreenBeforeHideSignal>(OnScreenBeforeHideSignal);
        signalBus.Subscribe<OnScreenBeforeShowSignal>(OnScreenBeforeShowSignal);
        signalBus.Subscribe<OnScreenAfterHideSignal>(OnScreenAfterHideSignal);
        signalBus.Subscribe<OnScreenAfterShowSignal>(OnScreenAfterShowSignal);
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

    private void OnScreenAfterShowSignal(OnScreenAfterShowSignal signal)
    {

    }

    private void OnScreenAfterHideSignal(OnScreenAfterHideSignal signal)
    {
        if (currentScreen == signal.Screen)
        {
            currentScreen = null;
        }
    }

    private void OnScreenBeforeShowSignal(OnScreenBeforeShowSignal signal)
    {
        if(currentScreen != null)
        {
            currentScreen.Hide();
        }
        currentScreen = signal.Screen;
    }

    private void OnScreenBeforeHideSignal(OnScreenBeforeHideSignal signal)
    {
        
    }
}