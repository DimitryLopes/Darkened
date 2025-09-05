using UnityEngine;

public class Entity : Activateable
{
    [SerializeField]
    private EntityStatusSO statusSO;

    protected EntityStatus status;


    public void Initialize()
    {
        status = new EntityStatus();
        status.Setup(statusSO);
    }
}
