using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterInspecterManager : MonoBehaviour
{
    [SerializeField] private GameObject[] characterModels;
    [SerializeField] private TMP_Text selectTxt, costTxt;
    [SerializeField] private Image selectImg, gemIcon;
    private int currentSelectedHero;
    private SelectType currentSelectType = SelectType.SELECT;

    private void Start()
    {
        currentSelectedHero = (int)GameFlowManager.Instance.UserProfile.selectedHero;
        ChangeDisplayModel(currentSelectedHero);
    }

    public void SetCharacterToInspect()
    {
        TurnOffAllModels();

        characterModels[(int)GameFlowManager.Instance.UserProfile.selectedHero].SetActive(true);
    }

    public void TurnOffAllModels()
    {
        for (int i = 0; i < characterModels.Length; i++)
        {
            characterModels[i].SetActive(false);
        }
    }

    public void ChangeDisplayModel(int order)
    {
        TurnOffAllModels();
        currentSelectedHero += order;
        if (currentSelectedHero >= characterModels.Length) currentSelectedHero = 0;
        else if (currentSelectedHero < 0) currentSelectedHero = characterModels.Length - 1;
        characterModels[currentSelectedHero].SetActive(true);
        ChangeSelectBtnFollowHero();
    }

    private void ChangeSelectBtnFollowHero()
    {
        CharacterInformation characterInformation = GameFlowManager.Instance.UserProfile.GetCharacterInformation((CharaterID)currentSelectedHero);
        costTxt.gameObject.SetActive(false);
        gemIcon.gameObject.SetActive(false);
        selectTxt.GetComponent<RectTransform>().offsetMax = new Vector2(0f, 0f);
        if(characterInformation.CharacterStatus == CharacterStatus.UNLOCKED)
        {
            selectTxt.text = "Select";
            selectImg.color = Color.white;
            currentSelectType = SelectType.SELECT;
        }

        if(characterInformation.CharacterStatus == CharacterStatus.LOCKED)
        {
            selectTxt.text = "Unlock";
            selectImg.color = Color.red;
            currentSelectType = SelectType.LOCK;
            costTxt.text = characterInformation.cost.ToString();
            costTxt.gameObject.SetActive(true);
            gemIcon.gameObject.SetActive(true);
            selectTxt.GetComponent<RectTransform>().offsetMax = new Vector2(0f, 105f);
        }

        if(GameFlowManager.Instance.UserProfile.selectedHero == (CharaterID)currentSelectedHero)
        {
            selectTxt.text = "Selected";
            selectImg.color = Color.green;
            currentSelectType = SelectType.SELECTED;
        }
    }

    public void SelectThisHero()
    {
        switch(currentSelectType) 
        {
            case SelectType.SELECT:
                    GameFlowManager.Instance.UserProfile.selectedHero = (CharaterID)currentSelectedHero;
                break;
            case SelectType.LOCK:
                    CharacterInformation characterInformation = GameFlowManager.Instance.UserProfile.GetCharacterInformation((CharaterID)currentSelectedHero);
                    if (GameFlowManager.Instance.UserProfile.EnoughCurrency(ItemType.DIAMOND, (int)characterInformation.cost))
                    {
                        GameFlowManager.Instance.UserProfile.UpdateCurrency(ItemType.DIAMOND, (int)-characterInformation.cost);
                        GameFlowManager.Instance.UserProfile.HeroUpdateStatus((CharaterID)currentSelectedHero, CharacterStatus.UNLOCKED);
                    }
                break;
        }
        ChangeSelectBtnFollowHero();
    }
}

public enum SelectType
{
    SELECT, SELECTED, LOCK
}
