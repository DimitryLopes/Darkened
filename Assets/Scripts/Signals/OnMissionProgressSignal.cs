public class OnMissionProgressSignal
{
    public IMission mission { get; private set; }
 
    public OnMissionProgressSignal(IMission mission)
    {
        this.mission = mission;
    }
}
