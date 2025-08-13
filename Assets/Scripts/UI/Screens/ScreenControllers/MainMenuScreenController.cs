public class MainMenuScreenController : ScreenController
{
    public LevelDataBase LevelDataBase { get; private set; }
    public System.Action<PresetLevelData> OnLevelViewClicked { get; set; }

    public MainMenuScreenController(LevelDataBase levelDataBase, 
        System.Action<PresetLevelData> onLevelViewClicked)
    {
        LevelDataBase = levelDataBase;
        OnLevelViewClicked = onLevelViewClicked;
    }
}
