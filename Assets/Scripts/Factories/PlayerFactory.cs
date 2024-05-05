using Zenject;
using UnityEngine;

public class PlayerFactory
{
    private readonly Player playerPrefab;
    private readonly DiContainer container;

    public PlayerFactory(DiContainer container, Player playerPrefab)
    {
        this.playerPrefab = playerPrefab;
        this.container = container;
    }

    public Player Create(Transform parent)
    {
        Player player = container.InstantiatePrefabForComponent<Player>(playerPrefab, parent);
        return player;
    }
}