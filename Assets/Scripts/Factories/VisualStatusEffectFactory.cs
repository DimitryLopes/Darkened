using Zenject;

public class VisualStatusEffectFactory
{
    private readonly VisualEntityStatusEffect visualPrefab;
    private readonly DiContainer container;

    public VisualStatusEffectFactory(DiContainer container, VisualEntityStatusEffect visualPrefab)
    {
        this.visualPrefab = visualPrefab;
        this.container = container;
    }

    public VisualEntityStatusEffect Create()
    {
        VisualEntityStatusEffect prefab = container.InstantiatePrefabForComponent<VisualEntityStatusEffect>(visualPrefab);
        return prefab;
    }
}