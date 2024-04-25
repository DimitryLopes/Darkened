using System;
using System.Reflection;
using UnityEngine;

public class UIDevScreen : UIScreen<DevScreenController>
{
    [SerializeField]
    public UIDevButton buttonPrefab;
    [SerializeField]
    public Transform buttonContainer;

    private void Start()
    {
        if (Controller.DeveloperTools == null)
        {
            Debug.LogError("Target script not set!");
            return;
        }

        Type targetType = Controller.DeveloperTools.GetType();
        MethodInfo[] methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

        foreach (MethodInfo method in methods)
        {
            UIDevButton button = Instantiate(buttonPrefab, buttonContainer);
            button.Init(method, Controller.DeveloperTools);
        }
    }
}
