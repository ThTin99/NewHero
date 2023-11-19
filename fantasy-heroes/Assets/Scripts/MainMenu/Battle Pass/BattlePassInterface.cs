using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattlePassInterface : MonoBehaviour
{
    [Header("Top Title")]
    [SerializeField] private TMP_Text seasonText;
    [SerializeField] private Button activateBtn;
    [SerializeField] private TMP_Text activateBtnText;
    [SerializeField] private TMP_Text seasonEndText;

    [Header("Mission Panel")]
    [SerializeField] private TMP_Text missionText;
    [SerializeField] private Image sliderFillMission;
    [SerializeField] private TMP_Text missionLevelText;

    [Header("Pass list")]
    [SerializeField] private BattlePassReward[] freePassRewards;
    [SerializeField] private Slider sliderPass;
    [SerializeField] private Image[] countBackground;
    [SerializeField] private BattlePassReward[] goldenPassRewards;

    [Header("Resources")]
    [SerializeField] private Sprite activeSliderCountBackground;
    [SerializeField] private Sprite graySliderCountBackground;

    [Header("Main Menu References")]
    [SerializeField] private TMP_Text mainMenuSeasonText;
    [SerializeField] private Image mainMenuSliderFillMission;
    [SerializeField] private TMP_Text mainMenumissionLevelText;


    private void Start()
    {
        InvokeRepeating(nameof(UpdateBattlePassTime), 0, 60);
        RefreshUI();
    }

    public void RefreshUI()
    {
        UpdateMainMenuBattlePassUI();
        UpdateMissionPanel();
        UpdateRewardPass();
        UpdateActiveGoldPassButtonAndApplyProcessingReward();
        UpdateSliderAndCountBackground();
    }

    private void UpdateMainMenuBattlePassUI()
    {
        mainMenuSeasonText.text = string.Format("<size=33><color=#0EEEF1>SEASON 1</color>  </size> {0} / {1}", GameFlowManager.Instance.UserProfile.battlePassLevel + 1, GameFlowManager.Instance.UserProfile.freeBattlePassRewardState.Length);

        mainMenumissionLevelText.text = (GameFlowManager.Instance.UserProfile.battlePassLevel + 1).ToString();

        mainMenuSliderFillMission.fillAmount = (float)GameFlowManager.Instance.UserProfile.currentBattlePassProgress / BattlePassManager.Instance.BattlePassDatabaseFree.battlePassQuests[GameFlowManager.Instance.UserProfile.battlePassLevel]._totalProgress;
    }

    private void UpdateSliderAndCountBackground()
    {
        sliderPass.value = GameFlowManager.Instance.UserProfile.battlePassLevel;

        for (int i = 0; i < countBackground.Length; i++)
        {
            if (i <= GameFlowManager.Instance.UserProfile.battlePassLevel)
            {
                countBackground[i].sprite = activeSliderCountBackground;
            }
            else
            {
                countBackground[i].sprite = graySliderCountBackground;
            }
        }
    }

    private void UpdateActiveGoldPassButtonAndApplyProcessingReward()
    {
        if (GameFlowManager.Instance.UserProfile.hasGoldenBattlePass)
        {
            activateBtn.interactable = false;
            activateBtnText.text = "Activated";

            if (GameFlowManager.Instance.UserProfile.currentBattlePassProgress >= BattlePassManager.Instance.BattlePassDatabaseFree.battlePassQuests[GameFlowManager.Instance.UserProfile.battlePassLevel]._totalProgress)//is ready to claim
            {
                goldenPassRewards[GameFlowManager.Instance.UserProfile.battlePassLevel].SetState(BattlePassReward.RewardState.PROCESSING);

                GameFlowManager.Instance.UserProfile.goldenBattlePassRewardState[GameFlowManager.Instance.UserProfile.battlePassLevel] = BattlePassReward.RewardState.PROCESSING;
            }
        }
        else
        {
            activateBtn.interactable = true;
            activateBtnText.text = "Activate";
        }

        if (GameFlowManager.Instance.UserProfile.currentBattlePassProgress >= BattlePassManager.Instance.BattlePassDatabaseFree.battlePassQuests[GameFlowManager.Instance.UserProfile.battlePassLevel]._totalProgress)//is ready to claim
        {
            freePassRewards[GameFlowManager.Instance.UserProfile.battlePassLevel].SetState(BattlePassReward.RewardState.PROCESSING);

            GameFlowManager.Instance.UserProfile.freeBattlePassRewardState[GameFlowManager.Instance.UserProfile.battlePassLevel] = BattlePassReward.RewardState.PROCESSING;
        }
    }

    private void UpdateRewardPass()
    {
        for (int i = 0; i < goldenPassRewards.Length; i++)
        {
            goldenPassRewards[i].Init(BattlePassManager.Instance.BattlePassDatabaseGold.battlePassQuests[i].rewardIcon, BattlePassManager.Instance.BattlePassDatabaseGold.battlePassQuests[i]._rewardTotal, GameFlowManager.Instance.UserProfile.goldenBattlePassRewardState[i]);
        }

        for (int i = 0; i < freePassRewards.Length; i++)
        {
            freePassRewards[i].Init(BattlePassManager.Instance.BattlePassDatabaseFree.battlePassQuests[i].rewardIcon, BattlePassManager.Instance.BattlePassDatabaseFree.battlePassQuests[i]._rewardTotal, GameFlowManager.Instance.UserProfile.freeBattlePassRewardState[i]);
        }
    }

    private void UpdateMissionPanel()
    {
        missionText.text = Lean.Localization.LeanLocalization.GetTranslationText(BattlePassManager.Instance.BattlePassDatabaseFree.battlePassQuests[GameFlowManager.Instance.UserProfile.battlePassLevel]._name);

        missionLevelText.text = (GameFlowManager.Instance.UserProfile.battlePassLevel + 1).ToString();

        sliderFillMission.fillAmount = (float)GameFlowManager.Instance.UserProfile.currentBattlePassProgress / BattlePassManager.Instance.BattlePassDatabaseFree.battlePassQuests[GameFlowManager.Instance.UserProfile.battlePassLevel]._totalProgress;
    }

    public void OnPurchaseGoldenBattlePass()
    {
        GameFlowManager.Instance.UserProfile.hasGoldenBattlePass = true;

        for (int i = 0; i < GameFlowManager.Instance.UserProfile.goldenBattlePassRewardState.Length; i++)
        {
            GameFlowManager.Instance.UserProfile.goldenBattlePassRewardState[i] = BattlePassReward.RewardState.PROCESSING;
        }

        UpdateRewardPass();
        UpdateActiveGoldPassButtonAndApplyProcessingReward();
    }

    private void UpdateBattlePassTime()
    {
        var now = DateTime.Now;
        seasonEndText.text = string.Format("{0} Days {1}H", 91 * ((now.DayOfYear / 91) + 1) - now.DayOfYear, 24 - now.Hour);//a season last 91.25 days
    }

    public void ClaimReward(int isGoldenPassReward)//0: free, 1: golden
    {
        if (isGoldenPassReward > 0)
        {
            for (int i = 0; i <= GameFlowManager.Instance.UserProfile.battlePassLevel; i++)
            {
                if (GameFlowManager.Instance.UserProfile.goldenBattlePassRewardState[i] == BattlePassReward.RewardState.PROCESSING)
                {
                    ClaimItemPopupManager.Instance.ShowTheClaimedItemWithAmount(BattlePassManager.Instance.BattlePassDatabaseGold.battlePassQuests[i].rewardIcon, BattlePassManager.Instance.BattlePassDatabaseGold.battlePassQuests[i].rewardType, BattlePassManager.Instance.BattlePassDatabaseGold.battlePassQuests[i]._rewardTotal);

                    GameFlowManager.Instance.UserProfile.goldenBattlePassRewardState[i] = BattlePassReward.RewardState.CLAIMED;
                    SoundManager.PlaySFX("Jingle_Achievement_00");
                }
            }
        }
        else
        {
            for (int i = 0; i <= GameFlowManager.Instance.UserProfile.battlePassLevel; i++)
            {
                if (GameFlowManager.Instance.UserProfile.freeBattlePassRewardState[i] == BattlePassReward.RewardState.PROCESSING)
                {
                    ClaimItemPopupManager.Instance.ShowTheClaimedItemWithAmount(BattlePassManager.Instance.BattlePassDatabaseFree.battlePassQuests[i].rewardIcon, BattlePassManager.Instance.BattlePassDatabaseFree.battlePassQuests[i].rewardType, BattlePassManager.Instance.BattlePassDatabaseFree.battlePassQuests[i]._rewardTotal);

                    GameFlowManager.Instance.UserProfile.freeBattlePassRewardState[i] = BattlePassReward.RewardState.CLAIMED;
                    SoundManager.PlaySFX("Jingle_Achievement_00");
                }
            }
        }

        UpdateProgressEventStatus();

        RefreshUI();
    }

    private static void UpdateProgressEventStatus()
    {
        if (GameFlowManager.Instance.UserProfile.currentBattlePassProgress >= BattlePassManager.Instance.BattlePassDatabaseFree.battlePassQuests[GameFlowManager.Instance.UserProfile.battlePassLevel]._totalProgress)//is quest completed
        {
            BattlePassManager.Instance.UnRegisterEvent();
            GameFlowManager.Instance.UserProfile.battlePassLevel++;
            GameFlowManager.Instance.UserProfile.currentBattlePassProgress = 0;
            BattlePassManager.Instance.RegisterEvent();
        }
    }
}
