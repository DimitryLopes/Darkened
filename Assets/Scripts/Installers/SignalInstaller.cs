using Zenject;

public class SignalInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        //In Game
        Container.DeclareSignal<OnGameCompletedSignal>();
        Container.DeclareSignal<OnMissionCompletedSignal>();
        Container.DeclareSignal<OnMazeLoadFinishSignal>();
        //Screens
        Container.DeclareSignal<OnScreenAfterHideSignal>();
        Container.DeclareSignal<OnScreenAfterShowSignal>();
        Container.DeclareSignal<OnScreenBeforeHideSignal>();
        Container.DeclareSignal<OnScreenBeforeShowSignal>();
        //UI
        Container.DeclareSignal<OnSelectableSelectedSignal>();
        //whatever armengue shit I did
        Container.DeclareSignal<OnBindingsFinishedSignal>();
    }
}