using System.Collections.Generic;

public class LevelSelectionScreenController : ScreenController
{
    public List<PresetLevelData> LevelDatas { get; private set; }
    public System.Action<PresetLevelData> OnLevelSelected { get; set; }

    public LevelSelectionScreenController(List<PresetLevelData> levelDatas, System.Action<PresetLevelData> onLevelSelected)
    {
        LevelDatas = levelDatas;
        OnLevelSelected = onLevelSelected;
    }
}
