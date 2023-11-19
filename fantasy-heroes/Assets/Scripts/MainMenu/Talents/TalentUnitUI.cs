using UnityEngine;
using UnityEngine.UI;

public class TalentUnitUI : MonoBehaviour
{
    [SerializeField] private Image m_TalentIcon;
    [SerializeField] private TMPro.TMP_Text m_TalentLevelText;
    private int m_Level;
    public TalentUnit m_TalentUnit;

    public void UpdateUI(TalentUnit talentUnit, int level)
    {
        m_TalentUnit = talentUnit;
        m_TalentIcon.sprite = m_TalentUnit.TalentIcon;
        m_Level = level;
        m_TalentLevelText.text = "Lv." + m_Level;
    }
}
