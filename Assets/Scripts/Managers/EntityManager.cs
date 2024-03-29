using System.Collections.Generic;
using UnityEngine;

public class EntityManager
{
    private readonly PlayerFactory playerFactory;
    private readonly EnemyFactory enemyFactory;

    private Dictionary<EnemyType, Enemy> Enemies = new Dictionary<EnemyType, Enemy>();
    private Player player;

    public EntityManager(PlayerFactory playerFactory, EnemyFactory enemyFactory)
    {
        this.playerFactory = playerFactory;
        this.enemyFactory = enemyFactory;
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

    public Enemy GetEnemy(EnemyType enemyType)
    {
        if (Enemies.ContainsKey(enemyType))
        {
            return Enemies[enemyType];
        }

        Enemy enemy = enemyFactory.Create(enemyType);
        Enemies.Add(enemyType, enemy);
        return enemy;
    }

    public void DeactivateEnemy(EnemyType enemyType)
    {
        if (Enemies.ContainsKey(enemyType))
        {
            Enemies[enemyType].Deactivate();
        }
        else
        {
            Debug.LogError($"Enemy of type: {enemyType} was not in the Database");
        }
    }

    public void ActivateEnemy(EnemyType enemyType)
    {
        if (Enemies.ContainsKey(enemyType))
        {
            Enemies[enemyType].Activate();
        }
        else
        {
            Debug.LogError($"Enemy of type: {enemyType} was not in the Database");
        }
    }
}
