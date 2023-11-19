using UnityEngine;
using UnityEngine.UI;

public class MaterialPopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMPro.TMP_Text m_ItemNameText;
    [SerializeField] private Image m_ItemIcon;
    [SerializeField] private TMPro.TMP_Text m_DescriptionText;

    public void ShowMaterialInfo(Item materialItem, int amount)
    {
        m_ItemNameText.text = materialItem.ItemName;
        m_ItemIcon.sprite = materialItem.ItemSprite;
        m_DescriptionText.text = $"Amount: {amount}";
        gameObject.SetActive(true);
    }
}
