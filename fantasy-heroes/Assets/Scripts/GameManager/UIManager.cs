using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Pause Menu Panel")]
    [SerializeField] private GameObject joyStick, pauseBtn;
    [SerializeField] private Lean.Gui.LeanWindow pausePanel, SliderPanelUI;
    [Header("Ability Chooser")]
    [SerializeField] private Lean.Gui.LeanWindow abilityPanel;
    [SerializeField] private AbilityCard[] abilityCards;
    [Header("Ability PauseMenu")]
    [SerializeField] private GameObject abilityCardsPauseMenuPrefab;
    [SerializeField] private Transform abilityCardsPauseMenuHolder;
    [Header("Lucky Wheel")]
    [SerializeField] private Lean.Gui.LeanWindow luckyWheelPanel;
    [Header("Lucky Wheel")]
    [SerializeField] private Lean.Gui.LeanWindow workshopPanel;
    [Header("Function Btn Pause Menu Panel")]
    //Volume Btn
    public GameObject soundOnImage;
    public GameObject soundOffImage;
    [Header("Misc")]
    [SerializeField] private Lean.Gui.LeanWindow transitionPanel;
    [SerializeField] private Lean.Gui.LeanWindow stageConclusionPanel;

    private void OnEnable()
    {
        EventManager.StartListening(EventManager.GameEventEnum.ON_LOAD_COMPLETED, () => { transitionPanel.TurnOff(); });
        EventManager.StartListening(EventManager.GameEventEnum.ON_TRANSITION_TO_NEXT_STAGE, () => { transitionPanel.TurnOn(); });
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventManager.GameEventEnum.ON_LOAD_COMPLETED, () => { transitionPanel.TurnOff(); });
        EventManager.StopListening(EventManager.GameEventEnum.ON_TRANSITION_TO_NEXT_STAGE, () => { transitionPanel.TurnOn(); });
    }

    public void PauseGame()
    {
        TimeManager.Instance.IgnoreUpdate = true;
        Time.timeScale = 0;
        pausePanel.TurnOn();
        pauseBtn.SetActive(false);
        joyStick.SetActive(false);
    }

    public void ResumeGame()
    {
        TimeManager.Instance.IgnoreUpdate = false;
        Time.timeScale = 1;
        pausePanel.TurnOff();
        pauseBtn.SetActive(true);
        joyStick.SetActive(true);
    }

    public void SettingBtn()
    {
        SliderPanelUI.TurnOn();
    }

    public void ShowAbilityPanel()
    {
        abilityPanel.TurnOn();
        foreach (var card in abilityCards)
        {
            card.CardData = AbilitiesManager.Instance.GetRandomAbility();
            card.UpdateCardInfo();
        }
    }

    public void ShowLuckyWheelPanel()
    {
        EventManager.TriggerEvent(EventManager.GameEventEnum.ON_LUCKY_WHEEL_APPREAR);
        luckyWheelPanel.TurnOn();
    }

    public void CloseLuckyWheelPanel()
    {
        luckyWheelPanel.TurnOff();
    }

    public void CloseAbilityPanel()
    {
        abilityPanel.TurnOff();
    }

    public void SpawnAbilityCardToPauseMenu(string _cardName, Sprite _cardSrite)
    {
        GameObject card = Instantiate(abilityCardsPauseMenuPrefab, abilityCardsPauseMenuHolder);
        card.transform.GetChild(0).GetComponent<Image>().sprite = _cardSrite;
    }

    public void ShowWorkShopPanel()
    {
        Time.timeScale = 0;
        workshopPanel.TurnOn();
        AbilitiesManager.Instance.UpdateWorkshopPanel();
    }

    public void CloseWorkShopPanel()
    {
        Time.timeScale = 1;
        workshopPanel.TurnOff();
    }

    private const string MUSICSETTINGKEY = "Music Setting";
    private const string SOUNDSETTINGKEY = "Sound Setting";

    private bool soundAllowed = false;

    public void UpdatePausePanelUI()
    {
        if (PlayerPrefs.HasKey(MUSICSETTINGKEY))
        {
            soundAllowed = PlayerPrefs.GetInt(MUSICSETTINGKEY) == 1;
        }
        if (PlayerPrefs.HasKey(SOUNDSETTINGKEY))
        {
            soundAllowed = PlayerPrefs.GetInt(SOUNDSETTINGKEY) == 1;
        }

        SetSoundIcon();
    }

    private void SetSoundIcon()
    {
        soundOnImage.SetActive(soundAllowed);
        soundOffImage.SetActive(!soundAllowed);
    }

    public void ToggleSFXSetting()
    {
        soundAllowed = !soundAllowed;
        SoundManager.SetDisableBGM(!soundAllowed);
        if (!soundAllowed)
        {
            SoundManager.StopMusic();
        }
        else
        {
            SoundManager.PlayConnection(SoundManager.GetSoundConnectionForThisLevel(SceneManager.GetActiveScene().name));
        }
        SoundManager.SetVolumeMusic(0.3f);
        SoundManager.SetDisableSFX(!soundAllowed);
        SetSoundIcon();
        PlayerPrefs.SetInt(SOUNDSETTINGKEY, soundAllowed ? 1 : 0);
        PlayerPrefs.SetInt(MUSICSETTINGKEY, soundAllowed ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void CountDownTimeOut()
    {
        stageConclusionPanel.TurnOn();
    }
}
