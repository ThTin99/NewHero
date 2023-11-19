using System.Collections;
using UnityEngine;

public class RevivePanelController : MonoBehaviour
{
    [SerializeField] private Lean.Gui.LeanWindow thisPanel;
    [SerializeField] private TMPro.TMP_Text countText;

    private void OnEnable()
    {
        EventManager.StartListening(EventManager.GameEventEnum.ON_HERO_DIE, () => { thisPanel.TurnOn(); });
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventManager.GameEventEnum.ON_HERO_DIE, () => { thisPanel.TurnOn(); });
    }

    private bool timeout = true;

    public void BringOurHeroBackToBattle()
    {
        timeout = false;
        EventManager.TriggerEvent(EventManager.GameEventEnum.ON_REVIVE_HERO);
    }

    public void StartCountDown(int second)
    {
        countText.text = second.ToString();
        StartCoroutine(CountDown(second));
    }

    private IEnumerator CountDown(int second)
    {
        do
        {
            yield return new WaitForSeconds(1f);
            second--;
            countText.text = second.ToString();
            SoundManager.PlaySFX("Button 4");
        } while (second > 0);

        if (second <= 0 && timeout)
        {
            thisPanel.TurnOff();
            UIManager.Instance.CountDownTimeOut();
        }

        timeout = true;
    }
}
