using Zenject;

public class Gate : MazeWall, IToggable
{
    private MazeNode node;
    private Cardinal alignedWith;

    public bool Toggled { get; private set; }

    public void Toggle(MazeManager mazeManager)
    {
        if(!Toggled)
        mazeManager.MazeGenerator.RemoveWall(node, alignedWith);
        else
        mazeManager.MazeGenerator.AddGate(node, alignedWith, this);
    }

    public override void OnWallCreated(SignalBus signalBus)
    {
        this.signalBus = signalBus;
    }

    internal void Setup(MazeNode availableNode, Cardinal alignedWith)
    {
        node = availableNode;
        this.alignedWith = alignedWith;
    }
}
