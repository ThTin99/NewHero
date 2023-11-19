using UnityEngine;

public class NotificationManager : Singleton<NotificationManager>
{
    [SerializeField] TMPro.TMP_Text notifyTxt;
    [SerializeField] Lean.Gui.LeanWindow thisPanel;

    public void ShowNotifyWithContent(string content)
    {
        notifyTxt.text = content.Trim();
        thisPanel.TurnOn();
    }
}