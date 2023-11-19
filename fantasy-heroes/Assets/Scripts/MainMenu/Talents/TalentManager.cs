using UnityEngine;

public class TalentManager : MonoBehaviour
{
    [SerializeField] private TalentUnitUI[] TalentUnitUIs;
    [SerializeField] private TMPro.TMP_Text upgradeCostText;

    private void Start()
    {
        for (int i = 0; i < TalentUnitUIs.Length; i++)
        {
            TalentUnit talentUnit = GameFlowManager.Instance.TalentDatabase.TalentUnits[i];
            TalentData talentData = GameFlowManager.Instance.UserProfile.talentDataList.Find(x => x.type == talentUnit.TalentType);
            if(talentData != null)
                TalentUnitUIs[i].UpdateUI(talentUnit, talentData.level);
        }
        uint m_UpgradeCost = (uint)1000 * GameFlowManager.Instance.UserProfile.TalentUpgradeTime;
        upgradeCostText.text = m_UpgradeCost.ToString();
    }

    public void UpgradeTalent()
    {
        uint m_UpgradeCost = (uint)1000 * GameFlowManager.Instance.UserProfile.TalentUpgradeTime;
        if (GameFlowManager.Instance.UserProfile.Gold >= m_UpgradeCost)
        {
            GameFlowManager.Instance.UserProfile.UpdateCurrency(ItemType.GOLD, (int)-m_UpgradeCost);
            var randIndex = Random.Range(0, GameFlowManager.Instance.UserProfile.talentDataList.Count);
            GameFlowManager.Instance.UserProfile.TalentUpdate(randIndex);
            TalentUnitUIs[randIndex].UpdateUI(TalentUnitUIs[randIndex].m_TalentUnit, GameFlowManager.Instance.UserProfile.talentDataList[randIndex].level);
            upgradeCostText.text = m_UpgradeCost.ToString();
        }
        else
        {
            NotificationManager.Instance.ShowNotifyWithContent("Insufficient gold!");
        }
    }
}

[System.Serializable]
public class TalentData
{
    public TalentType type;
    public int level;

    public TalentData(TalentType _type, int _level)
    {
        type = _type;
        level = _level;
    }
}
