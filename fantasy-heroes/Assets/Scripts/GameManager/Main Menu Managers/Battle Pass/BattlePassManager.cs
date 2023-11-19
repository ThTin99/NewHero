using UnityEngine;

public class BattlePassManager : Singleton<BattlePassManager>
{
    [SerializeField] internal BattlePassDatabase BattlePassDatabaseFree;
    [SerializeField] internal BattlePassDatabase BattlePassDatabaseGold;

    private void Start() {
        RegisterEvent();
    }

    public void RegisterEvent()
    {
        switch (BattlePassDatabaseFree.battlePassQuests[GameFlowManager.Instance.UserProfile.battlePassLevel]._type)
        {
            case BattlePassQuest.QuestType.KILL_MONSTER:
                EventManager.StartListening(EventManager.GameEventEnum.KILL_MONSTER, UpdateBattlePassProgress);
                break;
            case BattlePassQuest.QuestType.CLEAR_MAP:
                EventManager.StartListening(EventManager.GameEventEnum.CLEAR_MAP, UpdateBattlePassProgress);
                break;
            case BattlePassQuest.QuestType.PICKUP_GOLD:
                EventManager.StartListening(EventManager.GameEventEnum.PICKUP_GOLD, UpdateBattlePassProgress);
                break;
            case BattlePassQuest.QuestType.COMPLETE_CHAPTER:
                EventManager.StartListening(EventManager.GameEventEnum.COMPLETE_CHAPTER, SetBattlePassProgress);
                break;
            default:
                break;
        }
    }

    public void UnRegisterEvent()
    {
        switch (BattlePassDatabaseFree.battlePassQuests[GameFlowManager.Instance.UserProfile.battlePassLevel]._type)
        {
            case BattlePassQuest.QuestType.KILL_MONSTER:
                EventManager.StopListening(EventManager.GameEventEnum.KILL_MONSTER, UpdateBattlePassProgress);
                break;
            case BattlePassQuest.QuestType.CLEAR_MAP:
                EventManager.StopListening(EventManager.GameEventEnum.CLEAR_MAP, UpdateBattlePassProgress);
                break;
            case BattlePassQuest.QuestType.PICKUP_GOLD:
                EventManager.StopListening(EventManager.GameEventEnum.PICKUP_GOLD, UpdateBattlePassProgress);
                break;
            case BattlePassQuest.QuestType.COMPLETE_CHAPTER:
                EventManager.StopListening(EventManager.GameEventEnum.COMPLETE_CHAPTER, SetBattlePassProgress);
                break;
            default:
                break;
        }
    }

    private void UpdateBattlePassProgress(int progressAmount)
    {
        GameFlowManager.Instance.UserProfile.currentBattlePassProgress += progressAmount;
    }

    private void SetBattlePassProgress(int progressAmount)
    {
        if (progressAmount> GameFlowManager.Instance.UserProfile.currentBattlePassProgress)//only update new progress
        {
            GameFlowManager.Instance.UserProfile.currentBattlePassProgress = progressAmount;
        }
    }
}
