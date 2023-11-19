using UnityEngine;
using UnityEngine.UI;

public class ItemEntryUI : MonoBehaviour
{
    public Image IconeImage;
    public TMPro.TMP_Text ItemCount;
    public Button Button;

    public int InventoryEntry { get; set; } = -1;
    public EquipmentItem EquipmentItem { get; private set; }

    public InventoryUI Owner { get; set; }
    public int Index { get; set; }

    public void UpdateEntry()
    {
        try
        {
            InventorySystem.InventoryEntry entry = GameFlowManager.Instance.UserProfile.EquipmentItems[InventoryEntry];
            var item = EquipmentDatabaseManager.Instance.AllItems.Find(x => x.ItemName == entry.Item.ItemName);

            Button.onClick.AddListener(() => Owner.OnOpenItemInspector(this));

            IconeImage.gameObject.SetActive(true);
            IconeImage.sprite = item.ItemSprite;

            if (entry.Count > 1)
            {
                ItemCount.gameObject.SetActive(true);
                ItemCount.text = entry.Count.ToString();
            }
            else
            {
                ItemCount.gameObject.SetActive(false);
            }
        }
        catch (System.Exception)
        { }
    }

    public void SetupEquipment(EquipmentItem itm)
    {
        EquipmentItem = itm;

        enabled = itm != null;
        IconeImage.enabled = enabled;
        Button.interactable = enabled;
        if (enabled)
            IconeImage.sprite = EquipmentDatabaseManager.Instance.AllItems.Find(x => x.ItemName == itm.ItemName).ItemSprite;
    }

    public void InspectItem()
    {
        SoundManager.PlaySFX("Inventory_Open_00");
        Owner.OnOpenItemInspector(this, InventoryEntry == -1);
    }

    public void Equip()
    {
        if (InventoryEntry != -1)
        {
            Owner.EquipItem(GameFlowManager.Instance.InventorySystem.Entries[InventoryEntry]);
        }
        else
        {
            Owner.UnEquipItem(EquipmentItem);
        }
    }

    public void UpgradeItem()
    {
        if (InventoryEntry != -1)
        {
            Owner.UpgradeItem(GameFlowManager.Instance.InventorySystem.Entries[InventoryEntry]);
        }
        else
        {
            Owner.UpgradeItem(EquipmentItem);
        }
    }

    public void EnhanceItem()
    {
        if (InventoryEntry != -1)
        {
            Owner.EnhanceItem(GameFlowManager.Instance.InventorySystem.Entries[InventoryEntry]);
        }
        else
        {
            Owner.EnhanceItem(EquipmentItem);
        }
    }
}
