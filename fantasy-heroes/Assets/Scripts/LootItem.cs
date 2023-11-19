using UnityEngine;
using UnityEngine.UI;

public class LootItem : MonoBehaviour
{
    [SerializeField] internal Image icon;
    [SerializeField] internal TMPro.TMP_Text amountTxt;

    internal void UpdateDisplay(Sprite icon, uint amount)
    {
        this.icon.sprite = icon;
        amountTxt.text = "x" + amount.ToString();
    }
}
