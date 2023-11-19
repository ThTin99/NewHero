using UnityEngine;
using UnityEngine.UI;

public class OpenChestPopupManager : Singleton<OpenChestPopupManager>
{
    [SerializeField] private Lean.Gui.LeanWindow thisPanel;

    //[Header("Title")]
    //[SerializeField] private Image chestIcon;
    //[SerializeField] private TMPro.TMP_Text chestTitleTxt;

    [Header("Reward Info")]
    [SerializeField] private Image rewardIcon;
    [SerializeField] private TMPro.TMP_Text rewardTitleTxt;
    [SerializeField] private TMPro.TMP_Text rewardAmountTxt;

    [Header("Open Button")]
    [SerializeField] private TMPro.TMP_Text openTimeTxt;

    [Header("Resources")]
    [SerializeField] private Sprite goldIcon;
    [SerializeField] private Sprite diamondIcon;
    [SerializeField] private Sprite energyIcon;
    private SupplyChest currentChest;

    public void Init(SupplyChest chest)
    {
        currentChest = chest;
        TimedSupplyChest data = currentChest.data;
        switch (data.itemType)
        {
            case ItemType.GOLD:
                rewardIcon.sprite = goldIcon;
                rewardTitleTxt.text = "Gold";
                rewardTitleTxt.color = Color.yellow;
                break;
            case ItemType.DIAMOND:
                rewardIcon.sprite = diamondIcon;
                rewardTitleTxt.text = "Diamond";
                rewardTitleTxt.color = Color.magenta;
                break;
            case ItemType.ENERGY:
                rewardIcon.sprite = energyIcon;
                rewardTitleTxt.text = "Energy";
                rewardTitleTxt.color = Color.green;
                break;
            default:
                break;
        }

        rewardAmountTxt.text = string.Format("{0} - {1}", data.rewardAmount / 2, data.rewardAmount * 1.1f);
        openTimeTxt.text = "Start Unlock\n" + data.timeToOpen + "s";
        thisPanel.TurnOn();
    }

    public void OpenChest()
    {
        currentChest.StartOpenChest();
        thisPanel.TurnOff();
    }

    public void WatchAdsToOpenChest()
    {
        //TODO: USING ADMOD HERE
    }
}
