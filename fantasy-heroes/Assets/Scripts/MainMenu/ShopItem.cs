using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private int amount;
    [SerializeField] private TMP_Text amountTxt;
    [SerializeField] private ItemType ItemType;
    [SerializeField] private Image iconItem;
    [SerializeField] private Image iconBestValue;
    [SerializeField] private float price;
    [SerializeField] private TMP_Text priceTxt;
    [SerializeField] private ItemType priceType;
    [SerializeField] private bool isHardCoded;
    [SerializeField] private bool isBestChoice;
    [SerializeField] private string iapString;

    public void Setup(int amount, ItemType itemType, Sprite iconItem, float price, bool isBestChoice, string iapString)
    {
        this.amount = amount;
        amountTxt.text = amount.ToString();
        ItemType = itemType;
        this.iconItem.sprite = iconItem;
        this.price = price;
        iconBestValue.gameObject.SetActive(isBestChoice);
        if (TryGetComponent(out IAPButton iAPButton))
        {
            priceTxt.text = "$" + price;
            this.iapString = iAPButton.productId = iapString;
        }
        else
        {
            priceTxt.text = price.ToString();
            priceType = ItemType.DIAMOND;
        }
    }

    public void BuyCurrency()
    {
        switch (priceType)
        {
            case ItemType.DIAMOND:
                if (GameFlowManager.Instance.UserProfile.Diamond >= price)
                {
                    GetTheItem();
                }
                break;
            default:
                break;
        }
    }

    public void BuyWithCash()
    {
        SoundManager.PlaySFX("sfx_shop_purchase");
        GameFlowManager.Instance.UserProfile.AddItem(ItemType, UnitRank.COMMON, amount);
        ClaimItemPopupManager.Instance.ShowItemWithAmount(iconItem.sprite, amount);
        GameFlowManager.Instance.UserProfile.UpdateCurrency(priceType, 0);//call this to update the currency metter
    }

    private void GetTheItem()
    {
        SoundManager.PlaySFX("sfx_shop_purchase");
        GameFlowManager.Instance.UserProfile.AddItem(ItemType, UnitRank.COMMON, amount);
        ClaimItemPopupManager.Instance.ShowItemWithAmount(iconItem.sprite, amount);
        GameFlowManager.Instance.UserProfile.UpdateCurrency(priceType, -(int)price);
    }

    public void OpenChest()
    {
        var originalAmount = amount;
        amount = Random.Range(amount / 2, amount * 2);
        GetTheItem();
        amount = originalAmount;//reset the amount
    }
}
