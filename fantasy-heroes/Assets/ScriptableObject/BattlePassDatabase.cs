using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Battle Pass Database", menuName = "Database/Battle Pass")]
public class BattlePassDatabase : ScriptableObject
{
    public List<BattlePassQuest> battlePassQuests = new List<BattlePassQuest>();
}

[System.Serializable]
public struct BattlePassQuest
{
    public enum QuestType { KILL_MONSTER, CLEAR_MAP, PICKUP_GOLD, COMPLETE_CHAPTER }
    public string _name;
    public QuestType _type;
    public int _totalProgress;
    public ItemType rewardType;
    public Sprite rewardIcon;
    public int _rewardTotal;
}