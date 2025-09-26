using UnityEngine;
using System;

public class ElapsedTimeTriggerCondition : TutorialTriggerCondition
{
    public float Delay;
    private float startTime;

    private void Start()
    {
        startTime = Time.time;
    }

    public override bool IsMet()
    {
        return Time.time - startTime >= Delay;
    }
}