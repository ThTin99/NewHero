using UnityEngine;
using UnityEngine.UI;

public class BattlePassReward : MonoBehaviour
{
    private Image rewardIcon;
    private Image lockOrClaimIcon;
    private TMPro.TMP_Text amountText;
    private Button claimButton;

    public bool isGoldenPassReward = false;

    public enum RewardState { LOCKED, PROCESSING, CLAIMED }

    private RewardState rewardState;

    [Header("Resources")]
    [SerializeField] private Sprite lockSprite;
    [SerializeField] private Sprite checkSprite;

    private void Awake()
    {
        rewardIcon = transform.GetChild(1).GetComponent<Image>();
        amountText = transform.GetChild(2).GetComponent<TMPro.TMP_Text>();
        lockOrClaimIcon = transform.GetChild(3).GetComponent<Image>();
        claimButton = GetComponent<Button>();
    }

    public void Init(Sprite rewardIcon, int amount, RewardState rewardState)
    {
        this.rewardIcon.sprite = rewardIcon;
        amountText.text = amount.ToString();
        SetState(rewardState);
    }

    public void Refresh()
    {
        switch (rewardState)
        {
            case RewardState.LOCKED:
                lockOrClaimIcon.enabled = true;
                lockOrClaimIcon.sprite = lockSprite;
                claimButton.interactable = false;
                break;
            case RewardState.PROCESSING:
                lockOrClaimIcon.enabled = false;
                claimButton.interactable = true;
                break;
            case RewardState.CLAIMED:
                lockOrClaimIcon.enabled = true;
                lockOrClaimIcon.sprite = checkSprite;
                claimButton.interactable = false;
                break;
            default:
                break;
        }
    }

    public void SetState(RewardState rewardState)
    {
        this.rewardState = rewardState;
        Refresh();
    }
}
