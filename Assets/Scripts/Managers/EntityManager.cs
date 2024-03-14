public class EntityManager
{
    private readonly PlayerFactory playerFactory;
    private Player player;

    public EntityManager(PlayerFactory playerFactory)
    {
        this.playerFactory = playerFactory;
    }

    public Player GetPlayer()
    {
        if (player == null)
        {
            player = playerFactory.Create();
            player.SetUp();
        }

        return player;
    }
}
