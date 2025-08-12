using Zenject;

public class FloatingTextFactory
{
    private readonly FloatingText floatingTextPrefab;
    private readonly DiContainer container;

    public FloatingTextFactory(DiContainer container, FloatingText floatingTextPrefab)
    {
        this.floatingTextPrefab = floatingTextPrefab;
        this.container = container;
    }

    public FloatingText Create()
    {
        FloatingText floatingText = container.InstantiatePrefabForComponent<FloatingText>(floatingTextPrefab);
        return floatingText;
    }
}