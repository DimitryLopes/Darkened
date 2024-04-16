public class OnMissionCompletedSignal
{
    public IMission Mission { get; private set; }

    public OnMissionCompletedSignal(IMission mission)
    {
        Mission = mission;
    }
}
