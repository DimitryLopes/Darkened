using Zenject;

public class SignalInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<OnObjectiveCompletedSignal>();
        Container.DeclareSignal<OnMissionCompletedSignal>();

    }
}