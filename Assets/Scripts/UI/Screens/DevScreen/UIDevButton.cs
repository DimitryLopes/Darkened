using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDevButton : Button
{
    [SerializeField]
    public TextMeshProUGUI buttonText;

    private MethodInfo methodInfo;
    private object targetObject;

    public void Init(MethodInfo method, object target)
    {
        methodInfo = method;
        targetObject = target;
        buttonText.text = method.Name;
        onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        methodInfo.Invoke(targetObject, null);
    }
}
