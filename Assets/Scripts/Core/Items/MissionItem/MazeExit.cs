using Zenject;

public class MazeExit : MissionItem
{
    private SignalBus signalBus;

    [Inject]
    public void Create(SignalBus signalBus)
    {
        this.signalBus = signalBus;
    }
}


