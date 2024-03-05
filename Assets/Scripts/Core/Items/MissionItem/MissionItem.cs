using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionItem : Item
{
    protected Objective objective;

    public void SetObjective(Objective objective)
    {
        this.objective = objective;
    }

    public override void Interact()
    {
        if (canInteract)
        {
            ContributeToProgress();
        }
    }

    public virtual void ContributeToProgress()
    {
        
    }
}
