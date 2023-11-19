using BayatGames.SaveGameFree;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SupplyChestManager : Singleton<SupplyChestManager>
{
    public List<SupplyChestReward> chestsRewardInfo = new List<SupplyChestReward>();
    public List<SupplyChest> chestList = new List<SupplyChest>();
    [SerializeField] private SupplyChest chestPrefab;
    [SerializeField] private Transform chestHolder;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < GameFlowManager.Instance.UserProfile.chestList.Count; i++)
        {
            Spawn(GameFlowManager.Instance.UserProfile.chestList[i]);
        }
    }   

    private void Spawn(TimedSupplyChest data)
    {
        SupplyChest chest = Instantiate(chestPrefab, chestHolder);
        chest.Init(data);
        chestList.Add(chest);
    }

    public void SetChestsAvailableExcept(SupplyChest chest)
    {
        foreach(SupplyChest SC in chestList)
        {
            if(SC != chest)
            {
                SC.data.SetData(ChestState.AVAILABLE, DateTime.Now);
                SC.SetAvailable();
            }
        }
    }

    public void SetChestsLockedExcept(SupplyChest chest)
    {
        foreach(SupplyChest SC in chestList)
        {
            if(SC != chest)
            {
                SC.data.SetData(ChestState.LOCKED, DateTime.Now);
                SC.SetLocked();
            }
        }
    }

    public SupplyChestReward GetRewardIcon(ItemType itemType)
    {
        return chestsRewardInfo.Find(x => x.rewardID == itemType);
    } 

    public void ClearChestEmpty()
    {
        for(int i = chestList.Count - 1; i >= 0 ; i--)
        {
            if(chestList[i].chestState == ChestState.EMPTY)
                chestList.RemoveAt(i);
        }
    }
}

[System.Serializable]
public class TimedSupplyChest
{
    public int timeToOpen;//in second
    public ChestState chestState;
    public int quickOpenCost;
    public ItemType itemType;
    public int rewardAmount;
    public DateTime timeStamp;

    public TimedSupplyChest(int _timeToOpen, ChestState _chestState, int _quickOpenCost, ItemType _itemType, int _rewardAmount){
        timeToOpen = _timeToOpen;
        chestState = _chestState;
        quickOpenCost = _quickOpenCost;
        itemType = _itemType;
        rewardAmount = _rewardAmount;
    }

    public void SetData(ChestState chestState, DateTime timeStamp)
    {
        this.chestState = chestState;
        this.timeStamp = timeStamp;
    }
}

[System.Serializable]
public class SupplyChestReward
{
    public ItemType rewardID;
    public Sprite rewardIcon;
}