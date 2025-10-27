using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DeveloperToolsTrigger : MonoBehaviour
{
    [Inject]
    private DeveloperTools developerTools;
    [Inject]
    private ScreenManager screenManager;

    private Button button;
  
    private void Start()
    {
#if !UNITY_EDITOR
        gameObject.SetActive(false);
#endif
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }
    private void OnButtonClicked()
    {
        screenManager.Show<UIDevScreen>(new DevScreenController(developerTools));
    }
}
