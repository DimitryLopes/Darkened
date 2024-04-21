public interface IUnlockable
{
    UnlockConditionData UnlockConditionData { get; }

    public void Unlock();
}
