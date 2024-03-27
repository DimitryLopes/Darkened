public class OnMazeLoadStartedSignal
{
    public Maze Maze { get; private set; }

    public OnMazeLoadStartedSignal(Maze maze)
    {
        Maze = maze;
    }
}
