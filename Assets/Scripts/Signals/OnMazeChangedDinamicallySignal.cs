public class OnMazeChangedDinamicallySignal 
{
    public Node Node { get; private set; }
    public MazeWall Wall { get; private set; }

    public OnMazeChangedDinamicallySignal(Node node, MazeWall isWall)
    {
        Node = node;
        Wall = isWall;
    }
}
