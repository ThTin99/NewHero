using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyQuestInterface : Singleton<DailyQuestInterface>
{
    [Header("Title")]
    [SerializeField] private TMP_Text resetTimeTxt;

    [Header("Daily Progress bar")]
    [SerializeField] private TMP_Text totalDailyStarTxt;
    [SerializeField] private Image sliderProgress;

    [Header("Daily Chests")]
    [SerializeField] private DailyQuestChest[] dailyQuestChests;

    [Header("Quest Tab")]
    [SerializeField] private Transform questHolder;
    [SerializeField] private DailyQuestTab dailyQuestTabPrefab;
    [SerializeField] private List<DailyQuestTab> dailyQuestTabs;//claimed card not inclued

    [Header("Weekly Status")]
    [SerializeField] private TMP_Text totalWeeklyStarTxt;
    [SerializeField] private TMP_Text weeklyResetTimeTxt;

    [Header("Weekly Chests")]
    [SerializeField] private DailyQuestChest[] weeklyQuestChests;

    private bool isCreated;

    public void SetResetTimeText()
    {
        var now = DateTime.Now;
        resetTimeTxt.text = string.Format("{0:D1}D {1:D2}H", 0, 24 - now.Hour);//sunday: 0, saturday: 6
        weeklyResetTimeTxt.text = string.Format("{0:D1}D {1:D2}H", 6 - (int)now.DayOfWeek, 24 - now.Hour);//sunday: 0, saturday: 6
    }

    private void Start()
    {
        InvokeRepeating(nameof(SetResetTimeText), 0, 60 * 60);//update time once an hour
        Init();
    }

    private void OnDestroy()
    {
        CancelInvoke(nameof(SetResetTimeText));//to avoid possible bugs
    }

    private void Init()
    {
        UpdateStars();
        UpdateDailyChest();
        UpdateWeeklyChest();
        UpdateSliderBar();

        //create daily quest
        for (int i = 0; i < DailyQuestManager.Instance.questList.Count; i++)
        {
            var card = Instantiate(dailyQuestTabPrefab, questHolder);
            card.Init(DailyQuestManager.Instance.questList[i]);
            if (card != null)//claimed card will be destroyed so we have to check before add to the list
            {
                dailyQuestTabs.Add(card);
            }
        }

        isCreated = true;
    }

    private void UpdateSliderBar()
    {
        sliderProgress.fillAmount = GameFlowManager.Instance.UserProfile.dailyStar / 100f;
    }

    private void UpdateDailyChest()
    {
        for (int i = 0; i < dailyQuestChests.Length; i++)
        {
            dailyQuestChests[i].ChangeChestState(GameFlowManager.Instance.UserProfile.dailyChestState[i]);
            dailyQuestChests[i].RefreshUI();
        }
    }

    private void UpdateWeeklyChest()
    {
        for (int i = 0; i < weeklyQuestChests.Length; i++)
        {
            weeklyQuestChests[i].ChangeChestState(GameFlowManager.Instance.UserProfile.weeklyChestState[i]);
            weeklyQuestChests[i].RefreshUI();
        }
    }

    private void UpdateStars()
    {
        totalDailyStarTxt.text = GameFlowManager.Instance.UserProfile.dailyStar.ToString();
        totalWeeklyStarTxt.text = GameFlowManager.Instance.UserProfile.weeklyStar.ToString();
    }

    public void UpdateInfoOnEnableLeanWindow()
    {
        if (isCreated == true) UpdateInfo();
    }

    private void UpdateInfo()
    {
        DailyQuestManager.Instance.ResetDailyStar();
        UpdateStars();
        DailyQuestManager.Instance.SetChestState();
        UpdateSliderBar();
        UpdateDailyChest();
        UpdateWeeklyChest();
        RefreshQuestTabsUI();
    }

    private void RefreshQuestTabsUI()
    {
        DailyQuestManager.Instance.RefreshQuestData();

        for (int i = 0; i < dailyQuestTabs.Count; i++)
        {
            dailyQuestTabs[i].Init(DailyQuestManager.Instance.questList[i], true);
        }
    }

    public void CallSetLastOpenTimeInManager()
    {
        DailyQuestManager.Instance.SetLastOpenTime();
    }
}
