using UnityEngine;
using UnityEngine.UI;

public class ClaimItemPopupManager : Singleton<ClaimItemPopupManager>
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMPro.TMP_Text amountTxt;
    [SerializeField] private Lean.Gui.LeanWindow thisWindow;

    public void ShowTheClaimedItemWithAmount(Sprite icon, ItemType itemType, int amount)
    {
        itemIcon.sprite = icon;
        amountTxt.text = "x" + amount;
        GameFlowManager.Instance.UserProfile.UpdateCurrency(itemType, amount);
        thisWindow.TurnOn();
        SoundManager.PlaySFX("Jingle_Achievement_00");
    }

    public void ShowItemWithAmount(Sprite icon, int amount)
    {
        itemIcon.sprite = icon;
        amountTxt.text = "x" + amount;
        thisWindow.TurnOn();
        SoundManager.PlaySFX("Jingle_Achievement_00");
    }
}
