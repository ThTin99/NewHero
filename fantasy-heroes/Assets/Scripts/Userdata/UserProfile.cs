using BayatGames.SaveGameFree;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserProfile
{
    private const string Identifier = "Profile";
    public string userName;
    public string playerId;
    public int avatarIndex;
    public long exp;
    public int level;
    public int day;//use to sign in daily reward
    public CharaterID selectedHero;
    public int chapterIndex;//default
    public CampaignMode chapterMode;//default
    public List<TimedSupplyChest> chestList = new List<TimedSupplyChest>();
    public List<CharacterInformation> heroes = new List<CharacterInformation>();
    public List<MaterialsValue> materialValues = new List<MaterialsValue>();
    public List<EquipmentValue> equipments = new List<EquipmentValue>();
    public List<ItemInfo> items;
    public List<ProcessMapData> processMapDatas;
    public short dailyStar;
    public short weeklyStar;
    public DailyQuestChest.ChestState[] dailyChestState;
    public DailyQuestChest.ChestState[] weeklyChestState;
    public bool hasGoldenBattlePass;
    public short battlePassLevel;
    public int currentBattlePassProgress;
    public BattlePassReward.RewardState[] freeBattlePassRewardState;
    public BattlePassReward.RewardState[] goldenBattlePassRewardState;
    public int inventoryLimit;
    public int Level { get { return level; } set { level = value; } }
    public long Exp { get { return exp; } set { exp = value; } }
    public int Day { get { return day; } set { day = value; } }
    public List<InventorySystem.InventoryEntry> EquipmentItems;
    public Weapon Weapon;
    public EquipmentItem m_HeadSlot;
    public EquipmentItem m_TorsoSlot;
    public EquipmentItem m_LegsSlot;
    public EquipmentItem m_FeetSlot;
    public EquipmentItem m_AccessorySlot;
    public List<TalentData> talentDataList = new List<TalentData>();
    public ushort TalentUpgradeTime;

    public int Gold
    {
        get => items.Find(x => x._type == ItemType.GOLD)._total;
        set => items.Find(x => x._type == ItemType.GOLD)._total = value;
    }

    public int Diamond
    {
        get { return items.Find(x => x._type == ItemType.DIAMOND)._total; }
        set
        {
            var totalDiamond = items.Find(x => x._type == ItemType.DIAMOND);

            if (totalDiamond._total <= 0)
            {
                return;
            }
            else
            {
                totalDiamond._total = value;
            }
        }
    }

    public int Energy
    {
        get { return items.Find(x => x._type == ItemType.ENERGY)._total; }
        set { items.Find(x => x._type == ItemType.ENERGY)._total = value; }
    }

    public void ExtendInventory(int value)
    {
        inventoryLimit += value;
    }

    public string UserName { get { return userName; } set { userName = value; } }

    public UserProfile(string _playerID)
    {
        heroes = new List<CharacterInformation>();
        equipments = new List<EquipmentValue>();
        items = new List<ItemInfo>();
        materialValues = new List<MaterialsValue>();
        chestList = new List<TimedSupplyChest>();
        selectedHero = CharaterID.MaleArcher;

        TimedSupplyChest Chest1 = new TimedSupplyChest(60, ChestState.AVAILABLE, 5, ItemType.DIAMOND, 100);
        chestList.Add(Chest1);

        TimedSupplyChest Chest2 = new TimedSupplyChest(60, ChestState.AVAILABLE, 5, ItemType.GOLD, 10000);
        chestList.Add(Chest2);

        inventoryLimit = 32;
        items.Add(new ItemInfo(ItemType.GOLD, UnitRank.COMMON, 100000));
        items.Add(new ItemInfo(ItemType.DIAMOND, UnitRank.COMMON, 500));
        items.Add(new ItemInfo(ItemType.ENERGY, UnitRank.COMMON, 60));
        processMapDatas = new List<ProcessMapData>();

        for (int i = 0; i < 10; i++)
        {
            var normalProgress = new ProcessMapData.MapDetail(CampaignMode.NORMAL, 0);
            var eliteProgress = new ProcessMapData.MapDetail(CampaignMode.ELITE, 0);
            ProcessMapData.MapDetail[] mapDetails = { normalProgress, eliteProgress };
            var mapData = new ProcessMapData(i, mapDetails);
            processMapDatas.Add(mapData);
        }
        
        userName = "Avenger";
        playerId = _playerID;
        day = System.DateTime.Now.DayOfYear;
        exp = 0;
        level = 1;
        chapterIndex = 0;
        chapterMode = CampaignMode.NORMAL;
        dailyStar = 0;
        weeklyStar = 0;
        dailyChestState = new DailyQuestChest.ChestState[5] { DailyQuestChest.ChestState.Unclaimable, DailyQuestChest.ChestState.Unclaimable, DailyQuestChest.ChestState.Unclaimable, DailyQuestChest.ChestState.Unclaimable, DailyQuestChest.ChestState.Unclaimable };
        weeklyChestState = new DailyQuestChest.ChestState[3] { DailyQuestChest.ChestState.Unclaimable, DailyQuestChest.ChestState.Unclaimable, DailyQuestChest.ChestState.Unclaimable };
        hasGoldenBattlePass = false;
        battlePassLevel = 0;
        freeBattlePassRewardState = new BattlePassReward.RewardState[15];
        goldenBattlePassRewardState = new BattlePassReward.RewardState[15];
        currentBattlePassProgress = 0;

        EquipmentItems = new List<InventorySystem.InventoryEntry>();

        Weapon = null;
        m_HeadSlot = null;
        m_TorsoSlot = null;
        m_LegsSlot = null;
        m_FeetSlot = null;
        m_AccessorySlot = null;

        talentDataList = new List<TalentData>();
        for (int i = 0; i < GameFlowManager.Instance.TalentDatabase.TalentUnits.Count; i++)
        {
            TalentData talentData = new TalentData(GameFlowManager.Instance.TalentDatabase.TalentUnits[i].TalentType, 0);
            talentDataList.Add(talentData);
        }

        TalentUpgradeTime = 1;

        InitFreeBattlePass();
        InitGoldenBattlePass();
        AddEquipment(EquipmentId.BigBoy, 1);
        AddEquipment(EquipmentId.DoubleBlade, 1);
        AddMaterial(MaterialId.ShieldUpdate, 10);
        AddMaterial(MaterialId.SwordUpdate, 10);
        AddHero();
        Save();
    }

    public void UpdateDailyStar(short numberOfStars)
    {
        dailyStar += numberOfStars;
        Save();
    }

    public void UpdateWeeklyStar(short numberOfStars)
    {
        weeklyStar += numberOfStars;
        Save();
    }

    public void ChangeName(string _name)
    {
        userName = _name;
        Save();
    }

    public void ChangeAvatar(int index)
    {
        GameFlowManager.Instance.UserProfile.avatarIndex = index;
        Save();
    }

    public void TalentUpdate(int index)
    {
        TalentUpgradeTime++;
        talentDataList[index].level++;
        Save();
    }

    public void HeroUpdateStatus(CharaterID id, CharacterStatus characterStatus)
    {
        CharacterInformation characterInformation = heroes.Find(x => x.CharaterID.Equals(id));
        characterInformation.CharacterStatus = characterStatus;
        Save();
    }

    public void UpdateMapProgressData(int mapIndex, int mapDifficulty, int clearedStage)
    {
        if (processMapDatas[mapIndex].mapDetails[mapDifficulty].clearedstage < clearedStage)
        {
            processMapDatas[mapIndex].mapDetails[mapDifficulty].clearedstage = (uint)clearedStage;
        }
    }

    private void InitFreeBattlePass()
    {
        for (int i = 0; i < freeBattlePassRewardState.Length; i++)
        {
            freeBattlePassRewardState[i] = BattlePassReward.RewardState.LOCKED;
        }
    }

    private void InitGoldenBattlePass()
    {
        for (int i = 0; i < goldenBattlePassRewardState.Length; i++)
        {
            goldenBattlePassRewardState[i] = BattlePassReward.RewardState.LOCKED;
        }
    }

    internal void Save()
    {
        SaveGame.Save(Identifier, this);
    }

    public bool EnoughCurrency(ItemType type, int _value)
    {
        switch (type)
        {
            case ItemType.GOLD:
                if(Gold >= _value)
                    return true;
                break;

            case ItemType.DIAMOND:
                if(Diamond >= _value)
                    return true;
                break;

            case ItemType.ENERGY:
                if(Energy >= _value)
                    return true;
                break;
        }
        return false;
    }

    public void UpdateCurrency(ItemType type, int _value)
    {
        switch (type)
        {
            case ItemType.GOLD:
                Gold += _value;
                break;

            case ItemType.DIAMOND:
                Diamond += _value;
                //trigger daily quest
                if (_value < 0)
                {
                    EventManager.TriggerEvent(EventManager.GameEventEnum.USE_GEM, UnitQuest.QuestType.USE_GEMS, _value);
                }
                break;

            case ItemType.ENERGY:
                Energy += _value;
                //Use for exp reward
                if (_value < 0)
                {
                    GameFlowManager.Instance.ConsumedStaminaAmount = -_value;
                }
                break;
        }
        Save();
        EventManager.TriggerEvent(EventManager.GameEventEnum.ON_UPDATE_CURRENCY);
    }

    public CharacterInformation GetCharacterInformation(CharaterID id)
    {
        return heroes.Find(x => x.CharaterID.Equals(id));
    }

    public void AddEquipment(EquipmentId id, int level)
    {
        equipments.Add(new EquipmentValue(id, false, level));
    }

    public void AddMaterial(MaterialId id, int total)
    {
        if (materialValues.Exists(x => x.MaterialId.Equals(id)))
        {
            var item = materialValues.Find(x => x.MaterialId.Equals(id));
            item.Amount += total;
        }
        else
        {
            materialValues.Add(new MaterialsValue(id, total));
        }
    }

    public void AddItem(ItemType _type, UnitRank _rank, int _total, bool isUserMultipler = false)
    {
        if (items.Exists(x => x._type == _type && x._rank == _rank))
        {
            if (!isUserMultipler)
            {
                items.Find(x => x._type == _type && x._rank == _rank)._total += _total;
            }
            else
            {
                items.Find(x => x._type == _type && x._rank == _rank)._total = _total;
            }
        }
        else
        {
            ItemInfo info = new ItemInfo(_type, _rank, _total);
            items.Add(info);
        }
    }

    public void AddOrCreateItem(ItemType type, UnitRank rank, int quantity)
    {
        if (items.Exists(x => x._type.Equals(type)))
        {
            var item = items.Find(x => x._type.Equals(type));
            item._total = quantity;
        }
        else
        {
            ItemInfo item = new ItemInfo(type, rank, quantity);
            items.Add(item);
        }
    }

    public void AddHero()
    {
        CharacterInformation _hero = new CharacterInformation(CharaterID.MaleArcher, 1, CharacterStatus.UNLOCKED, 0);
        heroes.Add(_hero);

        CharacterInformation _hero1 = new CharacterInformation(CharaterID.MaleKnight, 1, CharacterStatus.LOCKED, 1000);
        heroes.Add(_hero1);

         CharacterInformation _hero2 = new CharacterInformation(CharaterID.FemaleArcher, 1, CharacterStatus.LOCKED, 2000);
        heroes.Add(_hero2);

        CharacterInformation _hero3 = new CharacterInformation(CharaterID.FemaleMage, 1, CharacterStatus.LOCKED, 3000);
        heroes.Add(_hero3);

        CharacterInformation _hero4 = new CharacterInformation(CharaterID.FemaleKnight, 1, CharacterStatus.LOCKED, 4000);
        heroes.Add(_hero4);

        CharacterInformation _hero5 = new CharacterInformation(CharaterID.MageMale, 1, CharacterStatus.LOCKED, 5000);
        heroes.Add(_hero5);
    }

    public void ClearChestEmpty()
    {
        for(int i = chestList.Count - 1; i >= 0 ; i--)
        {
            if(chestList[i].chestState == ChestState.EMPTY)
                chestList.RemoveAt(i);
        }
        Save();
    }

    //====PROCESS BUYING====//
    public bool OnProcessByGold(int _value)
    {
        if (Gold >= _value)
        {
            Gold -= _value;
            return true;
        }
        return false;
    }

    public bool OnProcessByDiamond(int _value)
    {
        if (Diamond >= _value)
        {
            Diamond -= _value;
            return true;
        }
        return false;
    }
    public bool OnProcessByEnergy(int _value)
    {
        if (Energy >= _value)
        {
            Energy -= _value;
            return true;
        }
        return false;
    }

    public string ChangeCurrency(int value)
    {
        string result = "0";
        if (value < 100000)
        {
            result = value.ToString();
        }
        if (value >= 100000 && value < 1000000)
        {
            value /= 1000;
            result = value + "K";
        }
        if (value >= 1000000)
        {
            value /= 1000000;
            result = value + "M";
        }
        return result;
    }

    public void ResetDailyStar()
    {
        dailyStar = 0;
    }

    #region Level Up Handler
    private void LevelUp(long additionalExp = 0)
    {
        level++;
        exp += additionalExp;
    }

    public void RewardExp(long amount)
    {
        //For levelup X then you need 25*X*(1+X) exp.
        var requiredExpAmount = GetRequiredExpAmount();
        if (amount > requiredExpAmount)
        {
            LevelUp(amount - requiredExpAmount);
        }
        else
        {
            exp += amount;
        }
    }

    public int GetRequiredExpAmount()
    {
        return 25 * level * (level + 1);
    }
    #endregion
}
