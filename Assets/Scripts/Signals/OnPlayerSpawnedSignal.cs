public class OnPlayerSpawnedSignal
{
    public Player Player { get; private set; }

    public OnPlayerSpawnedSignal(Player player)
    {
        Player = player;
    }
}
