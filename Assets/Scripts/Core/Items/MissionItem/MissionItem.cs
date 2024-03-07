using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionItem : Item
{
    protected Mission mission;

    public void SetMission(Mission mission)
    {
        this.mission = mission;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Interact();
        }
    }

    public override void Interact()
    {
        if (true)
        {
            ContributeToProgress();
        }
    }

    public virtual void ContributeToProgress()
    {
        mission.OnMissionProgress(type);
    }
}
