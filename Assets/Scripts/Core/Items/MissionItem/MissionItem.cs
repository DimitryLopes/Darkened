using UnityEngine;

public class MissionItem : Item
{
    protected Mission mission;

    public void SetMission(Mission mission)
    {
        this.mission = mission;
    }

    public override void Interact()
    {
        if (CanInteract)
        {
            signalBus.Fire(new OnMissionItemInteractedSignal(this));
            DisableInteraction();
        }
    }

    public void EnableInteraction()
    {
        canInteract = true;
    }

    public void DisableInteraction()
    {
        canInteract = false;
    }

}
