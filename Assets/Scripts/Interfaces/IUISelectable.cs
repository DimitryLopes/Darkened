public interface IUISelectable
{
    string Title { get; }
    SelectableType SelectableType { get; }
}

public enum SelectableType
{
    MazeSize,
    Difficulty,
    Objective,
    GameMode,
}
