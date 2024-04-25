using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTools
{
    private MazeManager mazeManager;
    private ObjectiveManager objectiveManager;

    public TestTools(MazeManager mazeManager, ObjectiveManager objectiveManager)
    {
        this.mazeManager = mazeManager;
        this.objectiveManager = objectiveManager;
    }

    public void CompleteCurrentObjective()
    {
        objectiveManager.CompleteCurrentObjective();
    }

    public void CompleteCurrentMission()
    {
        objectiveManager.CompleteCurrentMission();
    }

    public void ActivateAllTorches()
    {
        foreach (MazeTorch torch in mazeManager.CurrentMaze.Torches)
        {
            torch.ActivateLights();
        }
    }

    public void DeactivateAllTorches()
    {
        foreach (MazeTorch torch in mazeManager.CurrentMaze.Torches)
        {
            torch.DeactivateLights();
        }
    }
}
