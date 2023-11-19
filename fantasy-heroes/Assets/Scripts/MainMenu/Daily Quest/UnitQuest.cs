[System.Serializable]
public struct UnitQuest
{
    public enum State
    {
        PROCESSING, COMPLETED, COLLECTED
    }

    public enum QuestType
    {
        KILL_MONSTER, OPEN_GAMES, PLAY_GAMES, REACH_LEVEL_OF_HERO, TIME_PLAY_GAMES, WATCH_VIDEOS_ADS, CRAFT_ACCESSORIES, USE_SKIP, CLEAR_ALL_QUEST,
        UPGRADE_ALL_ITEMS_LV5, COMPLETE_CHAPTER, USE_GEMS, UPGRADE_10_TIMES
    }

    public string _name;
    public string key;
    public QuestType _type;
    public State state;
    public int _currentProgress, _totalProgress;
    public int _rewardTotal;

    public static UnitQuest GetFromJson(string jsons)
    {
        return UnityEngine.JsonUtility.FromJson<UnitQuest>(jsons);
    }

    public string SaveToString()
    {
        return UnityEngine.JsonUtility.ToJson(this);
    }

    public string GetJson()
    {
        return UnityEngine.PlayerPrefs.GetString(key, "");
    }

    public void Save()
    {
        UnityEngine.PlayerPrefs.SetString(key, SaveToString());
        UnityEngine.PlayerPrefs.Save();
    }
}