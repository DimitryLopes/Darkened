using System.Collections.Generic;
using UnityEngine;

public class EntityManager
{
    private readonly PlayerFactory playerFactory;
    private readonly EnemyFactory enemyFactory;
    private readonly EntityContainer container;

    private Dictionary<EnemyType, Enemy> Enemies = new Dictionary<EnemyType, Enemy>();
    private Player player;
    public EnemyType CurrentEnemyType { get; private set; } = EnemyType.None;

    public EntityManager(PlayerFactory playerFactory, EnemyFactory enemyFactory, EntityContainer container)
    {
        this.playerFactory = playerFactory;
        this.enemyFactory = enemyFactory;
        this.container = container;
    }

    public Player GetPlayer()
    {
        if (player == null)
        {
            player = playerFactory.Create(container.transform);
            player.SetUp();
        }

        return player;
    }

    public Enemy GetCurrentEnemy()
    {
        return GetEnemy(CurrentEnemyType);
    }

    public Enemy GetEnemy(EnemyType enemyType, bool setAsCurrent = false)
    {
        if(enemyType == EnemyType.None)
        {
            return null;
        }

        if (setAsCurrent)
        {
            CurrentEnemyType = enemyType;
        }

        if (Enemies.ContainsKey(enemyType))
        {
            return Enemies[enemyType];
        }

        Enemy enemy = enemyFactory.Create(enemyType, container.transform);
        Enemies.Add(enemyType, enemy);
        return enemy;
    }

    public void ToggleEntityActing(bool canAct)
    {
        foreach (var enemy in Enemies.Values)
        {
            enemy.CanAct = canAct;
        }
        player.ToggleActing(canAct);
    }

    public void DeactivateEnemy(EnemyType enemyType)
    {
        if (enemyType == EnemyType.None)
        {
            return;
        }

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
            Enemies[enemyType].Activate(true);
        }
        else
        {
            Debug.LogError($"Enemy of type: {enemyType} was not in the Database");
        }
    }
}
