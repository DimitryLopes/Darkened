using Zenject;

public class SignalInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        //In Game
        Container.DeclareSignal<OnGameCompletedSignal>();
        Container.DeclareSignal<OnNewGameStartedSignal>();
        Container.DeclareSignal<OnPlayerInteractableChangedSignal>();
        //Missions and Objectives
        Container.DeclareSignal<OnMissionProgressSignal>();
        Container.DeclareSignal<OnMissionCompletedSignal>();
        Container.DeclareSignal<OnMissionGroupStartedSignal>();
        Container.DeclareSignal<OnMissionGroupCompletedSignal>();
        Container.DeclareSignal<OnMissionItemInteractedSignal>();
        Container.DeclareSignal<OnEnemyHitSignal>();

        Container.DeclareSignal<OnMazeLoadFinishSignal>();
        Container.DeclareSignal<OnMazeLoadStartedSignal>();

        Container.DeclareSignal<OnPlayerExaustedRecoveredSignal>();
        Container.DeclareSignal<OnPlayerStaminaChangedSignal>();
        Container.DeclareSignal<OnPlayerStaminaExaustedSignal>();
        //Items
        Container.DeclareSignal<OnInventoryItemSelectedSignal>();
        Container.DeclareSignal<OnInventoryItemGetSignal>();
        //Screens
        Container.DeclareSignal<OnScreenAfterHideSignal>();
        Container.DeclareSignal<OnScreenAfterShowSignal>();
        Container.DeclareSignal<OnScreenBeforeHideSignal>();
        Container.DeclareSignal<OnScreenBeforeShowSignal>();
        //UI
        Container.DeclareSignal<OnSelectableSelectedSignal>();
        //HUD
        Container.DeclareSignal<OnInteractionButtonClickedSignal>();
        Container.DeclareSignal<OnBottomHUDNextButtonClickedSignal>();
        Container.DeclareSignal<OnPlayerItemUsedSignal>();
        Container.DeclareSignal<OnBottomHUDPreviousButtonClickedSignal>();
        //whatever armengue shit I did
        Container.DeclareSignal<OnBindingsFinishedSignal>();
    }
}