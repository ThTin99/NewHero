using UnityEngine;

public class UserCurrencyUpdater : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text diamondTxt;
    [SerializeField] private TMPro.TMP_Text goldTxt;

    // Start is called before the first frame update
    void Start()
    {
        UpdateCurrencyValue();
    }

    private void OnEnable()
    {
        EventManager.StartListening(EventManager.GameEventEnum.ON_UPDATE_CURRENCY, UpdateCurrencyValue);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventManager.GameEventEnum.ON_UPDATE_CURRENCY, UpdateCurrencyValue);
    }

    public void UpdateCurrencyValue()
    {
        diamondTxt.text = GameFlowManager.Instance.UserProfile.ChangeCurrency(GameFlowManager.Instance.UserProfile.Diamond);
        goldTxt.text = GameFlowManager.Instance.UserProfile.ChangeCurrency(GameFlowManager.Instance.UserProfile.Gold);
    }
}
