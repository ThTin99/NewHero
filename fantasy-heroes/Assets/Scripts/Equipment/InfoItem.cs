using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoItem : MonoBehaviour
{
    public TextMeshProUGUI NameItem, lv, dmg, health, armor, speed, crit;
    public Image imgItem, BGitem;
    public Button equipButton;
    public Button upgradeButton;
    public Sprite[] BGcolor;

    [SerializeField] private GameObject attackAttribute;
    [SerializeField] private GameObject healthAttribute;
    [SerializeField] private GameObject defendAttribute;
    [SerializeField] private GameObject agilityAttribute;
    [SerializeField] private GameObject critAttribute;
    [SerializeField] private TMP_Text upgradeCostTxt;
    [SerializeField] private Button enhanceButton;
    [SerializeField] private Transform m_RequiredMaterialHolder;
    [SerializeField] private ItemEntryUI materialPrefab;
    [SerializeField] private ParticleSystem[] upgradeVFXs;
    [SerializeField] private ParticleSystem[] enhanceVFXs;
    [SerializeField] private Image enhanceDimVFX;

    private EquipmentItem m_CurrentEquipment;
    private List<ItemEntryUI> m_MaterialList = new List<ItemEntryUI>();
    private int[] m_PreviousStats;
    private int[] m_CurrentStats;
    private int[] m_ChangeStats;

    public void SetInfor(DataItem value)
    { }

    private void ClearMaterialPrefabs()
    {
        m_MaterialList.Clear();

        for (int i = 0; i < m_RequiredMaterialHolder.childCount; i++)
        {
            Destroy(m_RequiredMaterialHolder.GetChild(i).gameObject);
        }
    }

    public void SetInfor(EquipmentItem equipment)
    {
        m_CurrentEquipment = equipment;

        var desc = m_CurrentEquipment.GetDescription().Split(new string[] { "\n" }, System.StringSplitOptions.None);
        m_PreviousStats = new int[desc.Length];
        for (int i = 0; i < desc.Length; i++)
        {
            m_PreviousStats[i] = desc[i] == "" ? 0 : int.Parse(desc[i]);
        }

        StatsHandle(equipment);
        RequiredMaterialHandle(equipment);
        IconHandle(equipment);
    }

    private void IconHandle(EquipmentItem equipment)
    {
        imgItem.sprite = EquipmentDatabaseManager.Instance.AllItems.Find(x => x.ItemName == equipment.ItemName).ItemSprite;

        switch (equipment.Rank)
        {
            case UnitRank.none:
                BGitem.sprite = BGcolor[0];
                break;
            case UnitRank.COMMON:
                BGitem.sprite = BGcolor[0];
                break;
            case UnitRank.GREAT:
                BGitem.sprite = BGcolor[1];
                break;
            case UnitRank.RARE:
                BGitem.sprite = BGcolor[2];
                break;
            case UnitRank.EPIC:
                BGitem.sprite = BGcolor[3];
                break;
            case UnitRank.LEGENDARY:
                BGitem.sprite = BGcolor[3];
                break;
            case UnitRank.MYSTICAL:
                BGitem.sprite = BGcolor[3];
                break;
        }
    }

    private void StatsHandle(EquipmentItem equipment)
    {
        NameItem.text = equipment.ItemName;
        lv.text = $"Lv. {equipment.Level}/{equipment.LevelLimit}";
        var desc = equipment.GetDescription().Split(new string[] { "\n" }, System.StringSplitOptions.None);
        dmg.text = desc[0];
        health.text = desc[1];
        armor.text = desc[2];
        speed.text = desc[3];
        crit.text = desc[4];

        switch (dmg.text)
        {
            case "0":
                attackAttribute.SetActive(false);
                break;
            default:
                attackAttribute.SetActive(true);
                break;
        }
        switch (health.text)
        {
            case "0":
                healthAttribute.SetActive(false);
                break;
            default:
                healthAttribute.SetActive(true);
                break;
        }
        switch (armor.text)
        {
            case "0":
                defendAttribute.SetActive(false);
                break;
            default:
                defendAttribute.SetActive(true);
                break;
        }
        switch (speed.text)
        {
            case "0":
                agilityAttribute.SetActive(false);
                break;
            default:
                agilityAttribute.SetActive(true);
                break;
        }
        switch (crit.text)
        {
            case "0":
                critAttribute.SetActive(false);
                break;
            default:
                critAttribute.SetActive(true);
                break;
        }
    }

    private void RequiredMaterialHandle(EquipmentItem equipment)
    {
        ClearMaterialPrefabs();

        var requirement = EquipmentDatabaseManager.Instance.EnhanceRequirements[equipment.Star - 1];
        for (int i = 0; i < requirement.RequiredMaterials.Count; i++)
        {
            var item = Instantiate(materialPrefab, m_RequiredMaterialHolder);
            item.IconeImage.sprite = requirement.RequiredMaterials[i].MaterialItem.ItemSprite;
            item.IconeImage.gameObject.SetActive(true);
            var availableAmount = GameFlowManager.Instance.UserProfile.EquipmentItems.Find(x => x.Item.ItemName == requirement.RequiredMaterials[i].MaterialItem.ItemName).Count;
            int requiredAmount = requirement.RequiredMaterials[i].Amount;
            var colorPrefix = "";
            var colorSubfix = "";
            if (availableAmount < requiredAmount)
            {
                colorPrefix = "<color=red>";
                colorSubfix = "</color>";
            }
            item.ItemCount.text = $"{colorPrefix}{availableAmount}{colorSubfix}/{requiredAmount}";
            item.ItemCount.gameObject.SetActive(true);
            m_MaterialList.Add(item);
        }
        upgradeCostTxt.text = equipment.CurrentUpgradeCost.ToString();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        m_CurrentEquipment = null;
    }

    public void SetEquipButton(ItemEntryUI hovered, bool isEquiped = false)
    {
        equipButton.onClick.RemoveAllListeners();
        equipButton.GetComponentInChildren<TMP_Text>().text = isEquiped ? "Unequip" : "Equip";
        equipButton.onClick.AddListener(() => { hovered.Equip(); gameObject.SetActive(false); });
    }

    public void SetUpgradeButton(ItemEntryUI hovered, bool isEquiped = false)
    {
        if (isEquiped ? hovered.EquipmentItem.IsReachLevelLimit : m_CurrentEquipment.IsReachLevelLimit)
        {
            upgradeButton.interactable = false;
        }
        else
        {
            upgradeButton.interactable = true;
        }
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(() => { hovered.UpgradeItem(); });
    }

    public void SetEnhanceButton(ItemEntryUI hovered, bool isEquiped = false)
    {
        if (isEquiped ? hovered.EquipmentItem.IsReachLevelLimit && hovered.EquipmentItem.CanEnhance && hovered.EquipmentItem.IsHaveEnoughMaterial() :
            m_CurrentEquipment.IsReachLevelLimit && m_CurrentEquipment.CanEnhance && m_CurrentEquipment.IsHaveEnoughMaterial())
        {
            enhanceButton.interactable = true;
        }
        else
        {
            enhanceButton.interactable = false;
        }
        enhanceButton.onClick.RemoveAllListeners();
        enhanceButton.onClick.AddListener(() => { hovered.EnhanceItem(); RefreshDisplay(); });
    }

    public void RefreshDisplay()
    {
        SetInfor(m_CurrentEquipment);
        upgradeButton.interactable = !m_CurrentEquipment.IsReachLevelLimit;
        enhanceButton.interactable = m_CurrentEquipment.IsReachLevelLimit && m_CurrentEquipment.CanEnhance;
    }

    public IEnumerator PlayUpgradeVFX()
    {
        upgradeButton.interactable = false;

        for (int i = 0; i < upgradeVFXs.Length; i++)
        {
            upgradeVFXs[i].Play();
        }

        var desc = m_CurrentEquipment.GetDescription().Split(new string[] { "\n" }, System.StringSplitOptions.None);
        m_ChangeStats = new int[desc.Length];
        m_CurrentStats = new int[desc.Length];
        for (int i = 0; i < desc.Length; i++)
        {
            m_CurrentStats[i] = desc[i] == "" ? 0 : int.Parse(desc[i]);
            m_ChangeStats[i] = (m_CurrentStats[i] - m_PreviousStats[i]) / 20;
        }
        float time = 1f;
        while (time > 0)
        {
            time -= 0.1f;
            dmg.text = $"<color=red>{m_PreviousStats[0] += m_ChangeStats[0]}</color>";
            health.text = $"<color=red>{m_PreviousStats[1] += m_ChangeStats[1]}</color>";
            armor.text = $"<color=red>{m_PreviousStats[2] += m_ChangeStats[2]}</color>";
            speed.text = $"<color=red>{m_PreviousStats[3] += m_ChangeStats[3]}</color>";
            crit.text = $"<color=red>{m_PreviousStats[4] += m_ChangeStats[4]}</color>";
            yield return new WaitForSeconds(0.1f);
        }

        RefreshDisplay();
    }

    public void PlayEnhanceVFX()
    {
        enhanceVFXs[0].Play();
        enhanceDimVFX.gameObject.SetActive(true);
        Invoke(nameof(PlaySecondEnhanceVFX), 0.5f);
    }

    private void PlaySecondEnhanceVFX()
    {
        enhanceVFXs[1].Play();
        enhanceDimVFX.gameObject.SetActive(false);
    }
}
