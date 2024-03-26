public class OnMissionProgressSignal
{
    public Mission mission { get; private set; }
 
    public OnMissionProgressSignal(Mission mission)
    {
        this.mission = mission;
    }
}
