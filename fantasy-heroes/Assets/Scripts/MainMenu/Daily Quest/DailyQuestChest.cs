using UnityEngine;
using UnityEngine.UI;

public class DailyQuestChest : MonoBehaviour
{
    [SerializeField] private GameObject glowVfx;
    [SerializeField] private GameObject claimIcon;
    [SerializeField] private Button button;
    [SerializeField] private Sprite rewardIcon;
    [SerializeField] private ItemType rewardType;
    [SerializeField] private int rewardAmount;
    [SerializeField] private bool isDailyChest;
    [SerializeField] private int chestIndex;

    public enum ChestState { Unclaimable, Claimable, Claimed }

    private ChestState chestState = ChestState.Unclaimable;

    public void ChangeChestState(ChestState chestState)
    {
        this.chestState = chestState;
        RefreshUI();
    }

    public void RefreshUI()
    {
        switch (chestState)
        {
            case ChestState.Unclaimable:
                glowVfx.SetActive(false);
                claimIcon.SetActive(false);
                button.interactable = false;
                break;
            case ChestState.Claimable:
                glowVfx.SetActive(true);
                claimIcon.SetActive(false);
                button.interactable = true;
                EventManager.TriggerEvent(EventManager.GameEventEnum.CAN_CLAIM_DAILY_REWARD);
                break;
            case ChestState.Claimed:
                glowVfx.SetActive(false);
                claimIcon.SetActive(true);
                button.interactable = false;
                break;
            default:
                break;
        }
    }

    private void Start()
    {
        button.onClick.AddListener(() =>
        {
            SoundManager.PlaySFX("Pickup_Gold_00");
            ClaimItemPopupManager.Instance.ShowTheClaimedItemWithAmount(rewardIcon, rewardType, rewardAmount);
            DailyQuestManager.Instance.SetChestState(chestIndex, isDailyChest, ChestState.Claimed);
            chestState = ChestState.Claimed;
            EventManager.TriggerEvent(EventManager.GameEventEnum.TURN_OFF_DAILY_CLAIMABLE_NOTIFY);
            RefreshUI();
        });
    }
}
