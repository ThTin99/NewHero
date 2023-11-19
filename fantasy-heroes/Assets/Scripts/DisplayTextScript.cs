using BayatGames.SaveGameFree;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayTextScript : MonoBehaviour
{
    [Header("Main Menu Panel")]
    [SerializeField] private TMP_Text mainMenuUsernameText;
    [SerializeField] private TMP_Text mainMenuLevelText, profileLevelTxt;
    [SerializeField] private TMP_Text mainMenuExpBarText;
    [SerializeField] private Slider mainMenuExpSlider;

    [Header("Main Top Panel")]
    [SerializeField] private Image avatar;
    [SerializeField] private TMP_Text userStatusText;

    [Header("Main Bot Panel")]
    [SerializeField] private TMP_Text heroClassText;
    [SerializeField] private TMP_Text heroStatusText;
    [SerializeField] private Image heroAvatar;

    [Header("Change Name Panel")]
    [SerializeField] private TMP_InputField changeNameText;
    [SerializeField] private Lean.Gui.LeanWindow changeNamePanel;

    [Header("Change Avatar Panel")]
    [SerializeField] private Sprite[] avatarResources;
    [SerializeField] private Image avatarCurrent;
    [SerializeField] private Image[] avatarObjs;

    private int selectedAvatarIndex = 0;

    void Start()
    {
        UpdateUserStatusText();

        for (int i = 0; i < avatarObjs.Length; i++)
        {
            avatarObjs[i].sprite = avatarResources[i];
        }

        selectedAvatarIndex = GameFlowManager.Instance.UserProfile.avatarIndex;
        avatar.sprite = heroAvatar.sprite = heroAvatar.sprite = avatarCurrent.sprite = avatarResources[selectedAvatarIndex];
    }

    private void UpdateExpBarText()
    {
        UserProfile userProfile = GameFlowManager.Instance.UserProfile;
        mainMenuExpBarText.text = $"{userProfile.Exp}/{userProfile.GetRequiredExpAmount()}";
        mainMenuExpSlider.value = (float)userProfile.exp / userProfile.GetRequiredExpAmount();
    }

    private void UpdateUserStatusText()
    {
        userStatusText.text = GameFlowManager.Instance.UserProfile.UserName;

        mainMenuUsernameText.text = GameFlowManager.Instance.UserProfile.UserName;
        mainMenuLevelText.text = GameFlowManager.Instance.UserProfile.Level.ToString();
        profileLevelTxt.text = "Lv." + GameFlowManager.Instance.UserProfile.Level.ToString();

        UpdateExpBarText();
    }

    public void ChangeName(int price)
    {
        if (string.IsNullOrEmpty(changeNameText.text.Trim()) || (changeNameText.text.Trim().Length > 16 && changeNameText.text.Trim().Length < 3))
        {
            NotificationManager.Instance.ShowNotifyWithContent("Username should not be empty!\nThe maximum characters 16.");
        }
        else
        {
            if (GameFlowManager.Instance.UserProfile.Diamond >= price)
            {
                GameFlowManager.Instance.UserProfile.UpdateCurrency(ItemType.DIAMOND, -price);
                GameFlowManager.Instance.UserProfile.ChangeName(changeNameText.text);
                UpdateUserStatusText();
                changeNamePanel.TurnOff();
            }
            else
            {
                NotificationManager.Instance.ShowNotifyWithContent("Not Enough Diamond!");
            }
        }
    }

    public void ChangeAvatar(int index)
    {
        avatar.sprite = heroAvatar.sprite = avatarCurrent.sprite = avatarResources[index];
        GameFlowManager.Instance.UserProfile.ChangeAvatar(index);
    }
}
