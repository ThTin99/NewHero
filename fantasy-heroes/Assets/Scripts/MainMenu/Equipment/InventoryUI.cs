using BayatGames.SaveGameFree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : Singleton<InventoryUI>
{
    public ItemEntryUI ItemEntryPrefab;
    public EquipmentUI EquipementUI;

    [SerializeField] private Transform holder;
    [SerializeField] private CharacterData currentCharacterData;
    [SerializeField] private InfoItem InfoItemPopup;
    [SerializeField] private MaterialPopup m_MaterialPopup;
    [SerializeField] private Lean.Gui.LeanWindow enhanceSuccessPopup;
    [SerializeField] private Image enhanceSuccessPopupIcon;
    [SerializeField] private TMPro.TMP_Text itemNameText;

    private List<ItemEntryUI> m_ItemEntries;
    private ItemEntryUI m_HoveredItem;
    private bool loaded = false;

    // public IEnumerator Start()
    // {
    //     GameFlowManager.Instance.InventorySystem.Init(currentCharacterData);
    //     if (!SaveGame.Exists("Test"))
    //     {
    //         //test
    //         for (int i = 1; i < EquipmentDatabaseManager.Instance.AllItems.Count; i++)
    //         {
    //             GameFlowManager.Instance.InventorySystem.AddItem(Instantiate(EquipmentDatabaseManager.Instance.AllItems[i]));
    //         }
    //         SaveGame.Save("Test", 1);
    //         //test
    //     }
    //     m_ItemEntries = new List<ItemEntryUI>();

    //     yield return new WaitUntil(() => currentCharacterData.Initiated);
    //     EquipementUI.Init(this);
    //     Load();
    //     loaded = true;
    // }

    public void Load()
    {
        if (loaded)
        {
            for (int i = 0; i < m_ItemEntries.Count; i++)
            {
                Destroy(m_ItemEntries[i].gameObject);
            }
            m_ItemEntries.Clear();
        }

        UserProfile userProfile = GameFlowManager.Instance.UserProfile;

        currentCharacterData.Stats = GameFlowManager.Instance.baseStatsHeroes[(int)userProfile.selectedHero];
        currentCharacterData.Stats.Init(currentCharacterData, true);

        EquipementUI.UpdateEquipment(currentCharacterData.Equipment, currentCharacterData.Stats);

        for (int i = 0; i < GameFlowManager.Instance.UserProfile.EquipmentItems.Count; ++i)
        {
            m_ItemEntries.Add(Instantiate(ItemEntryPrefab, holder));
            m_ItemEntries[i].Owner = this;
            m_ItemEntries[i].InventoryEntry = i;
            m_ItemEntries[i].UpdateEntry();
        }
    }

    public void OnOpenItemInspector(ItemEntryUI hovered, bool isEquiped = false)
    {
        m_HoveredItem = hovered;

        try
        {
            EquipmentItem itemUsed = (EquipmentItem)(m_HoveredItem.InventoryEntry != -1 ? GameFlowManager.Instance.InventorySystem.Entries[m_HoveredItem.InventoryEntry].Item : m_HoveredItem.EquipmentItem);

            InfoItemPopup.gameObject.SetActive(true);

            InfoItemPopup.SetInfor(itemUsed);
            InfoItemPopup.SetEquipButton(hovered, isEquiped);
            InfoItemPopup.SetUpgradeButton(hovered, isEquiped);
            InfoItemPopup.SetEnhanceButton(hovered, isEquiped);
        }
        catch (System.Exception)
        {
            InventorySystem.InventoryEntry inventoryEntry = GameFlowManager.Instance.InventorySystem.Entries[m_HoveredItem.InventoryEntry];
            m_MaterialPopup.ShowMaterialInfo(EquipmentDatabaseManager.Instance.AllItems.Find(x => x.ItemName == inventoryEntry.Item.ItemName), inventoryEntry.Count);
        }
    }

    public void EquipItem(InventorySystem.InventoryEntry usedItem)
    {
        GameFlowManager.Instance.InventorySystem.UseItem(usedItem);
        Load();
    }

    public void UnEquipItem(EquipmentItem equItem)
    {
        currentCharacterData.Equipment.Unequip(equItem.Slot);
        Load();
    }

    public void UpgradeItem(InventorySystem.InventoryEntry upgradeItem)
    {
        EquipmentItem item = upgradeItem.Item as EquipmentItem;

        if (GameFlowManager.Instance.UserProfile.Gold < item.CurrentUpgradeCost)
        {
            NotificationManager.Instance.ShowNotifyWithContent("Not enough Gold!");
        }
        else
        {
            item.Upgrade();
        }
    }

    public void UpgradeItem(EquipmentItem upgradeItem)
    {
        if (GameFlowManager.Instance.UserProfile.Gold < upgradeItem.CurrentUpgradeCost)
        {
            NotificationManager.Instance.ShowNotifyWithContent("Not enough Gold!");
        }
        else
        {
            upgradeItem.Upgrade();
        }
    }

    public void PlayUpgradeVFX()
    {
        StartCoroutine(InfoItemPopup.PlayUpgradeVFX());
    }

    public void EnhanceItem(InventorySystem.InventoryEntry enhanceItem)
    {
        EquipmentItem item = (EquipmentItem)enhanceItem.Item;
        if (GameFlowManager.Instance.UserProfile.Gold < item.CurrentUpgradeCost * 2)
        {
            NotificationManager.Instance.ShowNotifyWithContent("Not enough Gold!");
        }
        else
        {
            item.Enhance();
        }
    }

    /// <summary>
    /// Enhance cost = 2x current upgrade cost
    /// </summary>
    /// <param name="enhanceItem"></param>
    public void EnhanceItem(EquipmentItem enhanceItem)
    {
        if (GameFlowManager.Instance.UserProfile.Gold < enhanceItem.CurrentUpgradeCost * 2)
        {
            NotificationManager.Instance.ShowNotifyWithContent("Not enough Gold!");
        }
        else
        {
            enhanceItem.Enhance();
        }
    }

    public void ShowEnhancePopupSuccess(EquipmentItem equipmentItem)
    {
        StartCoroutine(ShowEnhancePopupSuccessWithDelay(equipmentItem));
    }

    IEnumerator ShowEnhancePopupSuccessWithDelay(EquipmentItem equipmentItem)
    {
        yield return new WaitForSeconds(0.85f);
        enhanceSuccessPopupIcon.sprite = EquipmentDatabaseManager.Instance.AllItems.Find(x => x.ItemName == equipmentItem.ItemName).ItemSprite;
        itemNameText.text = equipmentItem.ItemName;
        enhanceSuccessPopup.TurnOn();
    }

    public void ShowEnhanceVFX()
    {
        InfoItemPopup.PlayEnhanceVFX();
    }
}
