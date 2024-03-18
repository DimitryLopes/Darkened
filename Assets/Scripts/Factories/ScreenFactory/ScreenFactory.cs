using UnityEngine;
using Zenject;
using System;

public class ScreenFactory
{
    private readonly UIScreenDataBase screenDataBase;
    private readonly DiContainer container;
    private readonly DiContainer subContainer;
    private readonly MainCanvas mainCanvas;

    public ScreenFactory(DiContainer container, UIScreenDataBase screenDataBase, MainCanvas mainCanvas)
    {
        this.screenDataBase = screenDataBase;
        this.container = container;
        this.mainCanvas = mainCanvas;
        subContainer = container.CreateSubContainer();
    }

    public IScreen Create<T>() where T : IScreen
    {
        Type type = typeof(T);
        GameObject screenReference = screenDataBase.UIScreens[type];
        T newPrefab = container.InstantiatePrefabForComponent<T>(screenReference, mainCanvas.transform);

        subContainer.Inject(newPrefab);
        subContainer.ResolveRoots();
        
        return newPrefab;
    }
}