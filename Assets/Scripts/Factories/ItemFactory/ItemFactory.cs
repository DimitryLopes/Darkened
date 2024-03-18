using Zenject;

public class ItemFactory
{
    private readonly DiContainer container;
    private readonly ItemDataBase dataBase;

    public ItemFactory(ItemDataBase dataBase, DiContainer container)
    {
        this.container = container;
        this.dataBase = dataBase;
    }

    public Item Create(ItemType type)
    {
        Item prefab = dataBase.ItemDatas[type];
        return container.InstantiatePrefabForComponent<Item>(prefab);
    }
}