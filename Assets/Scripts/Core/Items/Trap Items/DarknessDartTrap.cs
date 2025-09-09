using System;
using UnityEngine;
using Zenject;

public class DarknessDartTrap : TrapItem
{
    [Inject]
    private MazeManager mazeManager;

    [SerializeField]
    private Sprite readySprite;
    [SerializeField]
    private Sprite notReadySprite;
    [SerializeField]
    private float triggerCooldown = 2;
    private float internalCooldown = 0;

    public override ItemType Type => ItemType.DarknessDartTrap;


    private void Update()
    {
        if (internalCooldown > 0)
        {
            internalCooldown -= Time.deltaTime;
            if(internalCooldown <= 0)
            {
                OnTriggerReady();
            }
        }
    }

    private void OnTriggerReady()
    {
        spriteRenderer.sprite = readySprite;
    }

    public override void Trigger()
    {
        if (internalCooldown > 0) return;

        var dart = mazeManager.GetAvailableItem(ItemType.DarknessDart) as DarknessDart;
        dart.transform.position = transform.position + transform.right/4;
        dart.Shoot(transform.right);
        spriteRenderer.sprite = notReadySprite;
        internalCooldown = triggerCooldown;
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        internalCooldown = 0;
    }
}
