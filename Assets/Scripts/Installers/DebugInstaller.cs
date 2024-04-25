using Zenject;
public class DebugInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<DeveloperTools>().AsSingle();

        SceneManager.LoadScene(Constants.Scenes.MAIN_MENU_SCENE);
    }
}
