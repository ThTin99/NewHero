using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Lean.Gui.LeanWindow[] windows;
    [SerializeField] private GameObject questRewardClaimableIcon;
    [Header("Bottom Icon")]
    [SerializeField] private Lean.Gui.LeanButton inventoryButton;
    [SerializeField] private GameObject inventoryLockImage;
    [SerializeField] private Lean.Gui.LeanButton talentButton;
    [SerializeField] private GameObject talentLockImage;
    [SerializeField] private Lean.Gui.LeanButton activityButton;
    [SerializeField] private GameObject activityLockImage;

    public void TurnWindowWithOrder(int order)
    {
        for (int i = 0; i < windows.Length; i++)
        {
            windows[i].TurnOff();
        }

        windows[order].TurnOn();
    }

    private void Start()
    {
        TurnOnWordPanel();

        if (GameFlowManager.Instance.UserProfile.Level < 0)
        {
            inventoryButton.interactable = false;
            inventoryLockImage.SetActive(true);
            talentButton.interactable = false;
            talentLockImage.SetActive(true);
            activityButton.interactable = false;
            activityLockImage.SetActive(true);
        }
        else if (GameFlowManager.Instance.UserProfile.Level < 0)
        {
            talentButton.interactable = false;
            talentLockImage.SetActive(true);
            activityButton.interactable = false;
            activityLockImage.SetActive(true);
        }
        else if (GameFlowManager.Instance.UserProfile.Level < 0)
        {
            activityButton.interactable = false;
            activityLockImage.SetActive(true);
        }
    }

    private void TurnOnWordPanel()
    {
        TurnWindowWithOrder(2);
    }

    private void OnEnable()
    {
        EventManager.StartListening(EventManager.GameEventEnum.CAN_CLAIM_DAILY_REWARD, () => { questRewardClaimableIcon.SetActive(true); });
        EventManager.StartListening(EventManager.GameEventEnum.TURN_OFF_DAILY_CLAIMABLE_NOTIFY, () => { questRewardClaimableIcon.SetActive(false); });
    }

    //Use SoundManager or attach to gameobject and call via LeanPlaySound
    public void PlayUISound(string soundName)
    {
        SoundManager.PlaySFX(soundName);
    }
}
