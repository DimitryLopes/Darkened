using UnityEngine;
using Zenject;

public class Darkness : MonoBehaviour
{
    [Inject]
    private SignalBus signalBus;
    [Inject]
    private GameManager gameManager;

    [SerializeField]
    private CreepyHand creepyHand;

    private float darknessTimer = 0f;
    private float maxDarknessTime;
    private Player player;

    private bool IsPlayerInDarkness => !player.IsLit;

    public void Start()
    {
        signalBus.Subscribe<OnMazeLoadFinishSignal>(OnMazeLoadFinish);
        signalBus.Subscribe<OnPlayerSpawnedSignal>(OnPlayerSpawned);
    }

    private void OnPlayerSpawned(OnPlayerSpawnedSignal signal)
    {
        darknessTimer = 0f;
        player = signal.Player;
    }

    private void OnMazeLoadFinish(OnMazeLoadFinishSignal signal)
    {
        maxDarknessTime = signal.Maze.Data.DifficultyData.MaxDarknessTime;
        creepyHand.SetDistance(0);
    }

    private void Update() 
    { 
        if (!gameManager.IsPlaying) return;

        if (IsPlayerInDarkness)
        {
            darknessTimer += Time.deltaTime;
            creepyHand.SetDistance(darknessTimer / maxDarknessTime);
            if (darknessTimer >= maxDarknessTime)
            {
                darknessTimer = maxDarknessTime;
                signalBus.Fire(new OnGameCompletedSignal(false));
            }
        }
        else if(darknessTimer > 0f)
        {
            darknessTimer -= Time.deltaTime;
            creepyHand.SetDistance(darknessTimer / maxDarknessTime);
            if(darknessTimer <= 0f)
            {
                darknessTimer = 0f;
            }
        }
    }
}
