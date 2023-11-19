using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnitQuest;

public class EventManager : MonoBehaviour
{
    public enum GameEventEnum { OPEN_GAME, PLAY_GAME, USE_GEM, COMPLETE_CHAPTER, COMPLETE_ALL, CAN_CLAIM_DAILY_REWARD, TURN_OFF_DAILY_CLAIMABLE_NOTIFY, KILL_MONSTER, CLEAR_MAP, PICKUP_GOLD, ON_LOAD_COMPLETED, ON_CLEARED_MAP, ON_BOSS_DEFEATED, ON_CLEARED_STAGE, ON_LUCKY_WHEEL_APPREAR, ON_BOSS_HEALTH_CHANGE, ON_BOSS_APPEARING, ON_REVIVE_HERO, ON_TRANSITION_TO_NEXT_STAGE, ON_HERO_DIE, ON_JOYSTICK_IDLE, ON_ENEMY_DIE, ON_UPDATE_CURRENCY, ON_OPEN_COMPLETED_CHEST };

    private Dictionary<GameEventEnum, UnityEvent> eventDictionary;

    private Dictionary<GameEventEnum, DailyQuestEvent> dailyQuestEeventDictionary;

    private Dictionary<GameEventEnum, BattlePassQuestEvent> battlePassQuestEeventDictionary;

    private static EventManager eventManager;

    public static EventManager Instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }

            return eventManager;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<GameEventEnum, UnityEvent>();
            dailyQuestEeventDictionary = new Dictionary<GameEventEnum, DailyQuestEvent>();
            battlePassQuestEeventDictionary = new Dictionary<GameEventEnum, BattlePassQuestEvent>();
        }
    }

    public static void StartListening(GameEventEnum eventName, UnityAction listener)
    {
        if (Instance.eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            Instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(GameEventEnum eventName, UnityAction listener)
    {
        if (eventManager == null) return;
        if (Instance.eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void StartListening(GameEventEnum eventName, UnityAction<QuestType, int> listener)
    {
        if (Instance.dailyQuestEeventDictionary.TryGetValue(eventName, out DailyQuestEvent thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new DailyQuestEvent();
            thisEvent.AddListener(listener);
            Instance.dailyQuestEeventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(GameEventEnum eventName, UnityAction<QuestType, int> listener)
    {
        if (eventManager == null) return;
        if (Instance.dailyQuestEeventDictionary.TryGetValue(eventName, out DailyQuestEvent thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }
    public static void StartListening(GameEventEnum eventName, UnityAction<int> listener)
    {
        if (Instance.battlePassQuestEeventDictionary.TryGetValue(eventName, out BattlePassQuestEvent thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new BattlePassQuestEvent();
            thisEvent.AddListener(listener);
            Instance.battlePassQuestEeventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(GameEventEnum eventName, UnityAction<int> listener)
    {
        if (eventManager == null) return;
        if (Instance.battlePassQuestEeventDictionary.TryGetValue(eventName, out BattlePassQuestEvent thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(GameEventEnum eventName)
    {
        if (Instance.eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent))
        {
            thisEvent.Invoke();
        }
    }

    public static void TriggerEvent(GameEventEnum eventName, QuestType questType, int amount)
    {
        if (Instance.dailyQuestEeventDictionary.TryGetValue(eventName, out DailyQuestEvent thisEvent))
        {
            thisEvent.Invoke(questType, amount);
        }
    }

    public static void TriggerEvent(GameEventEnum eventName, int amount)
    {
        if (Instance.battlePassQuestEeventDictionary.TryGetValue(eventName, out BattlePassQuestEvent thisEvent))
        {
            thisEvent.Invoke(amount);
        }
    }
}

[System.Serializable]
public class DailyQuestEvent : UnityEvent<QuestType, int>
{
}

[System.Serializable]
public class BattlePassQuestEvent : UnityEvent<int>
{
}