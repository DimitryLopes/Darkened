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
        Container.DeclareSignal<OnMazeChangedDinamicallySignal>();
        Container.DeclareSignal<OnTorchLitSignal>();
        Container.DeclareSignal<OnTorchAbsorbedSignal>();
        //Missions and Objectives
        Container.DeclareSignal<OnMissionProgressSignal>();
        Container.DeclareSignal<OnMissionCompletedSignal>();
        Container.DeclareSignal<OnMissionGroupStartedSignal>();
        Container.DeclareSignal<OnMissionGroupCompletedSignal>();
        Container.DeclareSignal<OnMissionItemInteractedSignal>();
        Container.DeclareSignal<OnEnemyHitSignal>();
        //Maze Generation
        Container.DeclareSignal<OnMazeLoadFinishSignal>();
        Container.DeclareSignal<OnMazeLoadStartedSignal>();
        Container.DeclareSignal<OnItemsLoadFinishSignal>();
        Container.DeclareSignal<OnPlayerSpawnedSignal>();
        //Player
        Container.DeclareSignal<OnPlayerExaustedRecoveredSignal>();
        Container.DeclareSignal<OnPlayerStaminaChangedSignal>();
        Container.DeclareSignal<OnPlayerStaminaExaustedSignal>();
        Container.DeclareSignal<OnTorchExtinguishedSignal>();
        //Enemy
        Container.DeclareSignal<OnEnemySpawnedSignal>();
        //Items
        Container.DeclareSignal<OnPlayerUsedItemSignal>();
        Container.DeclareSignal<OnPlayerTryToUseItemSignal>();
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
        Container.DeclareSignal<OnBottomHUDPreviousButtonClickedSignal>();
        //whatever armengue shit I did
        Container.DeclareSignal<OnBindingsFinishedSignal>();
    }
}