public interface IDataPersistence
{
    bool IsDirty { get; }
    void LoadData(GameData data);
    void SetDirty();
    void ResetDirty();
    void SaveData(ref GameData data);
    void SetPersistenceKey();
}
