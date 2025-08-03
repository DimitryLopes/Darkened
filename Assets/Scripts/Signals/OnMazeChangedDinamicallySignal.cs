public class OnMazeChangedDinamicallySignal 
{
    public MazeNode Node { get; private set; }
    public MazeWall Wall { get; private set; }

    public OnMazeChangedDinamicallySignal(MazeNode node, MazeWall isWall)
    {
        Node = node;
        Wall = isWall;
    }
}
