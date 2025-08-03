using Zenject;

public class Gate : MazeWall, IToggable
{
    private (Node,Node) nodes;

    public bool Toggled { get; private set; }

    public void Toggle(MazeManager mazeManager)
    {
        Toggled = !Toggled;
        if(!Toggled)
        mazeManager.MazeGenerator.RemoveWallBetween(nodes.Item1, nodes.Item2);
        else
        mazeManager.MazeGenerator.ActivateWallBetween(nodes.Item1, nodes.Item2);
    }

    public override void OnWallCreated(SignalBus signalBus)
    {
        this.signalBus = signalBus;
    }

    internal void Setup((Node, Node) availableNode, MazeWall subistitute)
    {
        nodes = availableNode;
        IsAtBorder = subistitute.IsAtBorder;
        transform.position = subistitute.transform.position;
        transform.rotation = subistitute.transform.rotation;
    }
}
