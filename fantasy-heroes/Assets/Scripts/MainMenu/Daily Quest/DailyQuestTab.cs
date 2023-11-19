using UnityEngine;
using UnityEngine.UI;

public class DailyQuestTab : MonoBehaviour
{
    [Header("Tab Background")]
    [SerializeField] private Image backgroundImg;

    [Header("Star")]
    [SerializeField] private TMPro.TMP_Text starValueTxt;

    [Header("Quest Title")]
    [SerializeField] private Image titleBackgroundImg;
    [SerializeField] private TMPro.TMP_Text questTitleTxt;

    [Header("Slider")]
    [SerializeField] private Image sliderFill;
    [SerializeField] private TMPro.TMP_Text progressTxt;

    [Header("Claim/Go Button")]
    [SerializeField] private Button claimButton;
    [SerializeField] private Image btnBackgroundImg;
    [SerializeField] private TMPro.TMP_Text buttonTxt;

    [Header("Resource References")]
    [SerializeField] private Sprite completeTabBackground;
    [SerializeField] private Sprite onGoingTabBackground;
    [SerializeField] private Sprite completeTitleBackground;
    [SerializeField] private Sprite onGoingTitleBackground;
    [SerializeField] private Sprite completeButtonBackground;
    [SerializeField] private Sprite onGoingButtonBackground;

    public UnitQuest UnitQuest;

    public void RefreshUI()
    {
        switch (UnitQuest.state)
        {
            case UnitQuest.State.PROCESSING:
                backgroundImg.sprite = onGoingTabBackground;
                titleBackgroundImg.sprite = onGoingTitleBackground;
                btnBackgroundImg.sprite = onGoingButtonBackground;
                buttonTxt.text = "Go";
                break;
            case UnitQuest.State.COMPLETED:
                backgroundImg.sprite = completeTabBackground;
                titleBackgroundImg.sprite = completeTitleBackground;
                btnBackgroundImg.sprite = completeButtonBackground;
                buttonTxt.text = "Claim";
                EventManager.TriggerEvent(EventManager.GameEventEnum.CAN_CLAIM_DAILY_REWARD);
                break;
            case UnitQuest.State.COLLECTED:
                gameObject.SetActive(false);
                break;
            default:
                break;
        }

        sliderFill.fillAmount = UnitQuest._currentProgress / ((float)UnitQuest._totalProgress);
        progressTxt.text = UnitQuest._currentProgress.ToString() + "/" + UnitQuest._totalProgress.ToString();
    }

    public void Init(UnitQuest unit, bool isUpdate = false)
    {
        UnitQuest = unit;
        RefreshUI();
        if (isUpdate)//dont add another listener when update state
        {
            return;
        }
        questTitleTxt.text = Lean.Localization.LeanLocalization.GetTranslationText(UnitQuest._name);
        starValueTxt.text = UnitQuest._rewardTotal.ToString();
        claimButton.onClick.AddListener(() =>
        {
            if (UnitQuest.state == UnitQuest.State.COMPLETED)
            {
                //TODO: ADD STAR TO TOTAL
                UnitQuest.state = UnitQuest.State.COLLECTED;
                UnitQuest.Save();
                //UI CLICK SOUND
                SoundManager.PlaySFX("Pickup_Gold_00");

                EventManager.TriggerEvent(EventManager.GameEventEnum.COMPLETE_ALL, UnitQuest.QuestType.CLEAR_ALL_QUEST, 1);//each completed quest will earn an clear all progress
                GameFlowManager.Instance.UserProfile.UpdateDailyStar((short)UnitQuest._rewardTotal);
                GameFlowManager.Instance.UserProfile.UpdateWeeklyStar((short)UnitQuest._rewardTotal);
                DailyQuestInterface.Instance.UpdateInfoOnEnableLeanWindow();//refresh UI
                EventManager.TriggerEvent(EventManager.GameEventEnum.TURN_OFF_DAILY_CLAIMABLE_NOTIFY);
                RefreshUI();
            }
            else
            {
                //UI CLICK SOUND
                SoundManager.PlaySFX("Button");
            }
        });
    }
}
