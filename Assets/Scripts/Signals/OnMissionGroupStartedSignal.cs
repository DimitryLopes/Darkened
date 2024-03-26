public class OnMissionGroupStartedSignal 
{
    public MissionGroup MissionGroup { get; private set; }

    public OnMissionGroupStartedSignal(MissionGroup group)
    {
        MissionGroup = group;
    }
}
