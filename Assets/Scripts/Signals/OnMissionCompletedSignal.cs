public class OnMissionCompletedSignal
{
    public Mission Mission { get; private set; }

    public OnMissionCompletedSignal(Mission mission)
    {
        Mission = mission;
    }
}
