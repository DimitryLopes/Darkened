public class OnMazeLoadFinishSignal
{
    public Maze Maze { get; private set; }

    public OnMazeLoadFinishSignal(Maze maze)
    {
        Maze = maze;
    }
}
