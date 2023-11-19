using UnityEngine;
using UnityEngine.UI;

public class MapProgressRewardItem : MonoBehaviour
{
    [Header("Refrences")]
    [SerializeField] private Image backgroundImg;
    [SerializeField] private Image[] icons;
    [SerializeField] private GameObject claimedIcon;

    [Header("Resources")]
    [SerializeField] private Sprite activeBackground;
    [SerializeField] private Sprite grayBackground;

    private Button button;

    public enum RewardState { LOCKED, AVAILABLE, CLAIMED }

    private RewardState rewardState = RewardState.LOCKED;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void UpdateUI()
    {
        switch (rewardState)
        {
            case RewardState.LOCKED:
                backgroundImg.sprite = grayBackground;
                claimedIcon.SetActive(false);
                button.interactable = false;
                break;
            case RewardState.AVAILABLE:
                backgroundImg.sprite = activeBackground;
                claimedIcon.SetActive(false);
                button.interactable = true;
                break;
            case RewardState.CLAIMED:
                backgroundImg.sprite = grayBackground;
                claimedIcon.SetActive(true);
                button.interactable = false;
                break;
            default:
                break;
        }
    }

    public void SetState(RewardState rewardState)
    {
        this.rewardState = rewardState;
    }

    public void Init(Sprite[] icons, RewardState rewardState)
    {
        for (int i = 0; i < icons.Length; i++)
        {
            this.icons[i].sprite = icons[i];
        }
        SetState(rewardState);
        UpdateUI();
    }
}
