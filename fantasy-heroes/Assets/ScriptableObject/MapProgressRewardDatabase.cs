using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map Progress Database", menuName = "Database/Map progress Database")]
public class MapProgressRewardDatabase : ScriptableObject
{
    public List<MapProgressReward> mapProgressRewards = new List<MapProgressReward>();
}

[System.Serializable]
public struct MapProgressReward
{
    public Sprite[] iconImgs;
    public ItemType[] rewards;
    public int[] rewardAmounts;
}