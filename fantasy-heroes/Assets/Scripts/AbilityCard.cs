using UnityEngine;
using UnityEngine.UI;

public class AbilityCard : MonoBehaviour
{
    private Ability cardData;
    [SerializeField] private TMPro.TMP_Text abilityName;
    [SerializeField] private TMPro.TMP_Text descriptionTxt;
    [SerializeField] private Image abilityIcon;

    public Ability CardData { get => cardData; set => cardData = value; }

    internal void SetInfo(Ability card)
    {
        cardData = card;
        UpdateCardInfo();
    }

    internal void UpdateCardInfo()
    {
        abilityName.text = cardData.abilityName;
        abilityIcon.sprite = cardData.abilityIcon;
    }

    public void ChooseMe(bool fromLuckyWheel = false)
    {
        ApplyCard();

        //dont set flag for ability chooser if this card from luck wheel
        if (fromLuckyWheel)
        {
            return;
        }
        GameManager.Instance.CloseAbilityChooseCanvasAndResumeTimeThenSetFlagCantChooseAbility();
    }

    private void ApplyCard(bool isWorkshopCard = false)
    {
        AbilitiesManager.Instance.AddAbility(this, isWorkshopCard);
        cardData.ApplyEffect();
        UIManager.Instance.SpawnAbilityCardToPauseMenu(abilityName.ToString(), abilityIcon.sprite);
        //lucky wheel dont have txt
        if (descriptionTxt != null)
        {
            ResetDescriptionTxt();
        }
        DamageUI.Instance.ShowAbilityDescription(Lean.Localization.LeanLocalization.GetTranslationText(cardData.abilityName), GameManager.Instance.abilityPopupPos.position);
    }

    public void ChooseMe()
    {
        ChooseMe(false);
    }

    public void ChooseThisWorkshopCard()
    {
        ApplyCard(true);
        UIManager.Instance.CloseWorkShopPanel();
    }

    public void ShowAbilityDescription()
    {
        descriptionTxt.text = Lean.Localization.LeanLocalization.GetTranslationText(cardData.abilityName);
    }

    private void ResetDescriptionTxt()
    {
        descriptionTxt.text = "";
    }
}
