using UnityEngine;
using Zenject;

public class Entity : Activateable
{
    [Inject]
    protected SignalBus signalBus;

    [SerializeField]
    private EntityStatusSO statusSO;
    [SerializeField]
    protected SpriteRenderer spriteRenderer;

    protected EntityStatus status;


    public void Initialize()
    {
        status = new EntityStatus();
        status.Setup(statusSO, signalBus);
    }
}
