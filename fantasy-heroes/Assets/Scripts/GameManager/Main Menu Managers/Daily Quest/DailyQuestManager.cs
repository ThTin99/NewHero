using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DailyQuestManager : Singleton<DailyQuestManager>
{
    private const string DayKey = "LastDayOpenTime";

    public List<UnitQuest> questList;//all available quest

    [SerializeField] DataQuest DataQuest;

    private int m_LastOpenTime;

    private IEnumerator Start()
    {
        Load();
        yield return new WaitUntil(() => EventManager.Instance != null);
        EventManager.TriggerEvent(EventManager.GameEventEnum.OPEN_GAME, UnitQuest.QuestType.OPEN_GAMES, 1);
    }

    public void OnCheckQuest(UnitQuest.QuestType type, int _value)
    {
        var array = questList.FindAll(x => x._type == type);
        for (int i = 0; i < array.Count; i++)
        {
            UnitQuest x = array[i];
            if (x._currentProgress < x._totalProgress)
            {
                x._currentProgress += _value;

                if (x._currentProgress >= x._totalProgress)
                {
                    x.state = UnitQuest.State.COMPLETED;
                    x._currentProgress = x._totalProgress;
                }

                x.Save();
            }
        }
        RefreshQuestData();
    }

    public void Load()
    {
        ResetDailyStar();

        RefreshQuestData();

        SetChestState();
    }

    public void ResetDailyStar()
    {
        if (PlayerPrefs.HasKey(DayKey))
        {
            if (DateTime.Now.DayOfYear != PlayerPrefs.GetInt(DayKey))
            {
                GameFlowManager.Instance.UserProfile.ResetDailyStar();

                for (int i = 0; i < questList.Count; i++)
                {
                    UnitQuest x = questList[i];
                    x._currentProgress = 0;
                    x.state = UnitQuest.State.PROCESSING;
                    x.Save();
                }
            }
        }
    }

    public void RefreshQuestData()
    {
        questList = new List<UnitQuest>();

        foreach (var x in DataQuest.dailyQuest)
        {
            string str = x.GetJson();

            if (str == "") questList.Add(x.DeepClone());//something wrong here => need to reset
            else
            {
                var temp = UnitQuest.GetFromJson(x.GetJson());
                questList.Add(temp);
            }
        }
    }

    private void UpdateWeeklyChestsState()
    {
        for (int i = 0; i < GameFlowManager.Instance.UserProfile.weeklyStar / 200; i++)//maximum 700 stars, 3 chest, 200 stars per chest, minus 1 to have index => the formula is divided by interval
        {
            if (GameFlowManager.Instance.UserProfile.weeklyChestState[i] != DailyQuestChest.ChestState.Claimed)
            {
                GameFlowManager.Instance.UserProfile.weeklyChestState[i] = DailyQuestChest.ChestState.Claimable;
            }
        }
    }

    private void UpdateDailyChestsState()
    {
        for (int i = 0; i < GameFlowManager.Instance.UserProfile.dailyStar / 20; i++)//maximum 100 stars, 5 chest, 20 stars per chest, minus 1 to have index => the formula is divided by interval
        {
            if (GameFlowManager.Instance.UserProfile.dailyChestState[i] != DailyQuestChest.ChestState.Claimed)
            {
                GameFlowManager.Instance.UserProfile.dailyChestState[i] = DailyQuestChest.ChestState.Claimable;
            }
        }
    }

    private void OnApplicationQuit()
    {
        //Save the current system time as a string in the player prefs class
        PlayerPrefs.SetString("exitTime", DateTime.Now.ToBinary().ToString());
    }

    public void SetChestState(int chestIndex, bool isDailyChest, DailyQuestChest.ChestState chestState)
    {
        if (isDailyChest)
        {
            GameFlowManager.Instance.UserProfile.dailyChestState[chestIndex] = chestState;
        }
        else
        {
            GameFlowManager.Instance.UserProfile.weeklyChestState[chestIndex] = chestState;
        }
    }

    public void SetChestState()
    {
        UpdateDailyChestsState();
        UpdateWeeklyChestsState();
    }

    private void OnEnable()
    {
        EventManager.StartListening(EventManager.GameEventEnum.OPEN_GAME, OnCheckQuest);
        EventManager.StartListening(EventManager.GameEventEnum.COMPLETE_CHAPTER, OnCheckQuest);
        EventManager.StartListening(EventManager.GameEventEnum.PLAY_GAME, OnCheckQuest);
        EventManager.StartListening(EventManager.GameEventEnum.USE_GEM, OnCheckQuest);
        EventManager.StartListening(EventManager.GameEventEnum.COMPLETE_ALL, OnCheckQuest);
    }

    public void SetLastOpenTime()
    {
        PlayerPrefs.SetInt(DayKey, DateTime.Now.DayOfYear);
        PlayerPrefs.Save();
    }
}

public static class CloneHelper
{
    public static T DeepClone<T>(this T obj)
    {
        var ms = new MemoryStream();
        var formatter = new BinaryFormatter();
        formatter.Serialize(ms, obj);
        ms.Position = 0;

        return (T)formatter.Deserialize(ms);
    }
}