public class OnMissionGroupCompletedSignal 
{
    public MissionGroup MissionGroup { get; private set; }

    public OnMissionGroupCompletedSignal(MissionGroup group)
    {
        MissionGroup = group;
    }
}
