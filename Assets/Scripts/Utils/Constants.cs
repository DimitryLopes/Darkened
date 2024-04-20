using System.IO;
using UnityEngine;

public static class Constants
{
    public const string HORIZONTAL_AXIS = "Horizontal";
    public const string VERTICAL_AXIS = "Vertical";

    public static class Scenes
    {
        public const string LOADING_SCENE = "Loading Scene";
        public const string MAIN_MENU_SCENE = "Main Menu";
        public const string TESTING_SCENE = "Testing";
        public const string GAME_SCENE = "Game";
    }

    public static class Levels
    {
        public const string LEVEL_SAVED_DATA_KEY = "level_{0}";
    }

    public static class Hud
    {
        public const string UI_MISSION_DESCRIPTION_FORMAT = "{0}: {1}/{2}";
    }

    public static class Save
    {
        public static string PERSISTENCE_FILE_PATH =  Path.Combine(Application.persistentDataPath, PERSISTENCE_FILE_NAME);
        public const string PERSISTENCE_FILE_NAME = "GameData.json";
        public const string PERSISTENCE_LEVEL_KEY_FORMAT = "Level_{0}";
    }

    public static class LayersAndTags
    {
        public const string WALL_TAG = "Wall";
        public const string PLAYER_TAG = "Player";
        public const string ENEMY_TAG = "Enemy";
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


