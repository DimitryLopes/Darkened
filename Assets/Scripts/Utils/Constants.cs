using System.IO;
using UnityEngine;

public static class Constants
{
    public static class UI 
    {
        public const int LEVEL_SELECTION_SCREEN_LEVEL_COUNT_PER_PAGE =10;
    }

    public static class Player
    {
        public const float PLAYER_DEFAULT_TORCH_SIZE = 1;
        public const float PLAYER_BIG_TORCH_SIZE = 1.6f;
        public const float PLAYER_SMALL_TORCH_SIZE = 0.75f;
        public const float TORCH_LIFETIME = 20;
        public const string STATUS_EFFECTS_KEY_FORMAT = "S{0}D{1}M{2}";
    }

    public static class Scenes
    {
        public const string LOADING_SCENE = "Loading Scene";
        public const string MAIN_MENU_SCENE = "Main Menu";
        public const string TESTING_SCENE = "Testing";
        public const string GAME_SCENE = "GeneratedGame";
    }

    public static class Levels
    {
        public const string LEVEL_SAVED_DATA_KEY = "level_{0}";
    }

    public static class Hud
    {
        public const string UI_MISSION_DESCRIPTION_FORMAT = "{0}: {1}/{2}";
    }

    public static class Generation
    {
        public const string WALL_ID_FORMAT = "[{0},{1}] [{2},{3}]";
    }

    public static class Save
    {
        public static string PERSISTENCE_FILE_PATH =  Path.Combine(Application.persistentDataPath, PERSISTENCE_FILE_NAME);
        public const string PERSISTENCE_FILE_NAME = "GameData.json";
        public const string PERSISTENCE_LEVEL_KEY_FORMAT = "Level_{0}";
        public const string PLAYER_SFX_KEY = "sfx_pref_key";
        public const string PLAYER_BGM_KEY = "bgm_pref_key";
    }

    public static class LayersAndTags
    {
        public const string WALL_TAG = "Wall";
        public const string PLAYER_TAG = "Player";
        public const string ENEMY_TAG = "Enemy";
        public const string TORCH_TAG = "Torch";
        public const string WALL_ITEM_TAG = "Wall Item";
        public const string NODE_ITEM_TAG = "Node Item";
        public const string WALL_ITEM_REPLACE_TAG = "Wall Item Replace";
        public const string NODE_ITEM_REPLACE_TAG = "Node Item Replace";
    }

    public static class AudioParameters
    {
        public const string MIXER_GROUP_VOLUME_PARAMETER = "_Volume";
    }
}


