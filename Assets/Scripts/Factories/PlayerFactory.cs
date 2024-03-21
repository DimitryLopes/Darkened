using Zenject;

public class PlayerFactory
{
    private readonly Player playerPrefab;
    private readonly DiContainer container;

    public PlayerFactory(DiContainer container, Player playerPrefab)
    {
        this.playerPrefab = playerPrefab;
        this.container = container;
    }

    public Player Create()
    {
        Player player = container.InstantiatePrefabForComponent<Player>(playerPrefab);
        return player;
    }
}