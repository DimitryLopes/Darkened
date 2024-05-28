using System;
using System.Reflection;
using TMPro;
using UnityEngine;

public class UIDevScreen : UIScreen<DevScreenController>
{
    [SerializeField]
    public UIDevButton buttonPrefab;
    [SerializeField]
    public Transform buttonContainer;
    [SerializeField]
    private TextMeshProUGUI fpsText;

    private float deltaTime = 0.0f;

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

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = Mathf.Ceil(fps).ToString() + " FPS";
    }
}
