using UnityEngine;
using UnityEngine.UI;

public class BossWarningPanelController : MonoBehaviour
{
    [SerializeField] private Animation Animation;
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private Slider Slider;
    [SerializeField] private Lean.Gui.LeanWindow thisPanel;

    public void PlayWarningSplashAnimation()
    {
        Animation.Play();
    }

    public void HideWaringPanelAfter2s()
    {
        Invoke(nameof(HideWarningPanel), 3f);
    }

    public void UpdateHpBar(int value)
    {
        Slider.value = value;
    }

    private void HideWarningPanel()
    {
        warningPanel.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.StartListening(EventManager.GameEventEnum.ON_BOSS_HEALTH_CHANGE, UpdateHpBar);
        EventManager.StartListening(EventManager.GameEventEnum.ON_BOSS_DEFEATED, () => { thisPanel.TurnOff(); });
        EventManager.StartListening(EventManager.GameEventEnum.ON_BOSS_APPEARING, () => { thisPanel.TurnOn(); UpdateHpBar(100); });
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventManager.GameEventEnum.ON_BOSS_HEALTH_CHANGE, UpdateHpBar);
        EventManager.StopListening(EventManager.GameEventEnum.ON_BOSS_DEFEATED, () => { thisPanel.TurnOff(); });
        EventManager.StopListening(EventManager.GameEventEnum.ON_BOSS_APPEARING, () => { thisPanel.TurnOn(); UpdateHpBar(100); });
    }
}
