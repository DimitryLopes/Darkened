using Zenject;

public class Gate : MazeWall, IToggable
{
    private (Node,Node) nodes;
    public ShadowCasterHelper ShadowHelper { get; set; }

    public bool Toggled => IsActive;

    public void Toggle(MazeManager mazeManager)
    {
        if(Toggled)
        mazeManager.MazeGenerator.RemoveWallBetween(nodes.Item1, nodes.Item2);
        else
        mazeManager.MazeGenerator.ActivateWallBetween(nodes.Item1, nodes.Item2);

        signalBus.Fire(new OnMazeChangedDinamicallySignal(null, this));
    }

    public void Setup((Node,Node) nodes)
    {
        this.nodes = nodes;
    }

    public void Setup((Node, Node) nodes, MazeWall subistitute)
    {
        Setup(nodes);
        IsAtBorder = subistitute.IsAtBorder;
        transform.position = subistitute.transform.position;
        transform.rotation = subistitute.transform.rotation;
    }
}
