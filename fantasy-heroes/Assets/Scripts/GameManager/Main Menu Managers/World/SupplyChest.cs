using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SupplyChest : MonoBehaviour
{
    public TimedSupplyChest data;
    [SerializeField] private GameObject empty;
    [Header("Locked Chest")]
    [SerializeField] private GameObject locked;
    [SerializeField] private TMP_Text lockTxt;
    [Header("Active Chest")]
    [SerializeField] private GameObject opening;
    [SerializeField] private int timeToOpen;
    [SerializeField] private Button chestBtn;
    [SerializeField] private TMP_Text timeTxt;
    [SerializeField] private TMP_Text timeLockedTxt;
    [SerializeField] private TMP_Text openTxt;
    [SerializeField] private GameObject groupCost;
    [SerializeField] private TMP_Text costTxt;

    [SerializeField] internal DateTime timeStamp;
    [SerializeField] internal ChestState chestState = ChestState.EMPTY;

    public int TimeToOpen { get => timeToOpen; set => timeToOpen = value; }
    public TMP_Text CostTxt { get => costTxt; set => costTxt = value; }

    // Update is called once per frame
    void Update()
    {
        if (chestState == ChestState.OPENING)
        {
            if (IsChestComplete())
            {
                SetCompleted();
            }
        }
    }

    private bool IsChestComplete()
    {
        TimeSpan difference = GetTimeDifference();

        // If the counter below 0 it means there is a new reward to claim
        if (difference.TotalSeconds <= 0)
        {
            return true;
        }
        else
        {
            string formattedTs = GetFormattedTime(difference);
            timeTxt.text = string.Format(formattedTs);
            return false;
        }
    }

    public string GetFormattedTime(TimeSpan span)
    {
        return string.Format("{0:D2}:{1:D2}:{2:D2}", span.Hours, span.Minutes, span.Seconds);
    }

    public TimeSpan GetTimeDifference()
    {
        var targetTime = timeStamp.AddSeconds(TimeToOpen);
        return targetTime.Subtract(DateTime.Now);
    }

    public void Init(TimedSupplyChest _data)
    {
        data = _data;
        TimeToOpen = data.timeToOpen;
        CostTxt.text = data.quickOpenCost.ToString();
        timeStamp = data.timeStamp;

        switch (data.chestState)
        {
            case ChestState.EMPTY:
                SetEmpty();
                break;
            case ChestState.LOCKED:
                SetLocked();
                break;
            case ChestState.OPENING:
                SetOpening();
                break;
            case ChestState.COMPLETED:
                SetCompleted();
                break;
            case ChestState.AVAILABLE:
                SetAvailable();
                break;
        }
    }

    public void OpenChest()
    {
        switch (chestState)
        {
            case ChestState.AVAILABLE:
                OpenChestPopupManager.Instance.Init(this);
                SoundManager.PlaySFX("Inventory_Open_00");
                break;
            case ChestState.OPENING:
                if (GameFlowManager.Instance.UserProfile.Diamond >= data.quickOpenCost)
                {
                    //TODO: SHOW YES NO PANEL
                    GameFlowManager.Instance.UserProfile.UpdateCurrency(ItemType.DIAMOND, data.quickOpenCost);
                    ChestCompletedAction();
                }
                else
                {
                    NotificationManager.Instance.ShowNotifyWithContent("Not Enough Diamond!");
                }
                break;
            case ChestState.COMPLETED:
                ChestCompletedAction();
                break;
        }
    }

    private void ChestCompletedAction()
    {
        SoundManager.PlaySFX("Pickup_Gold_00");
        ClaimItemPopupManager.Instance.ShowTheClaimedItemWithAmount(SupplyChestManager.Instance.GetRewardIcon(data.itemType).rewardIcon, 
                                                                    data.itemType, 
                                                                    data.rewardAmount);
        data.SetData(ChestState.EMPTY, DateTime.Now);
        SetEmpty();
        SupplyChestManager.Instance.SetChestsAvailableExcept(this);
    }

   

    public void StartOpenChest()
    {
        SoundManager.PlaySFX("Spell_00");
        timeStamp = DateTime.Now;
        data.SetData(ChestState.OPENING, timeStamp);
        SetOpening();
        SupplyChestManager.Instance.SetChestsLockedExcept(this);
    }

    public void SetLocked()
    {
        empty.SetActive(false);
        opening.SetActive(false);
        locked.SetActive(true);
        chestBtn.interactable = false;
        chestState = ChestState.LOCKED;
        timeLockedTxt.text = timeToOpen + "s";
    }

    internal void SetAvailable()
    {
        chestBtn.interactable = true;
        chestState = ChestState.AVAILABLE;
        openTxt.text = Lean.Localization.LeanLocalization.GetTranslationText("Open Phrase");
        timeTxt.text = timeToOpen + "s";
        groupCost.SetActive(false);
        opening.SetActive(true);
        locked.SetActive(false);
    }

    internal void SetCompleted()
    {
        chestBtn.interactable = true;
        chestState = ChestState.COMPLETED;
        openTxt.text = Lean.Localization.LeanLocalization.GetTranslationText("Open Phrase");
        groupCost.SetActive(false);
    }

    public void SetEmpty()
    {
        empty.SetActive(true);
        opening.SetActive(false);
        locked.SetActive(false);
        chestBtn.interactable = false;
        chestState = ChestState.EMPTY;
        GameFlowManager.Instance.UserProfile.ClearChestEmpty();
        SupplyChestManager.Instance.ClearChestEmpty();
        Destroy(gameObject);
    }

    public void SetOpening()
    {
        empty.SetActive(false);
        opening.SetActive(true);
        chestState = ChestState.OPENING;
        locked.SetActive(false);
        groupCost.SetActive(true);
        openTxt.text = Lean.Localization.LeanLocalization.GetTranslationText("Open Now Phrase");
    }
}

public enum ChestState { EMPTY, AVAILABLE, LOCKED, OPENING, COMPLETED }