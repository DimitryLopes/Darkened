using System;
using UnityEngine;
using Zenject;

public class PoisonDartTrap : TrapItem
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

        var dart = mazeManager.GetAvailableItem(ItemType.PoisonDart) as PoisonDart;
        dart.transform.position = transform.position;
        dart.Shoot(transform.right);
        spriteRenderer.sprite = notReadySprite;
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        internalCooldown = 0;
    }
}
