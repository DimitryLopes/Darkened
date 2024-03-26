public class OnMissionItemInteractedSignal
{
    public MissionItem MissionItem { get; private set; }

    public OnMissionItemInteractedSignal(MissionItem item)
    {
        MissionItem = item;
    }
}
