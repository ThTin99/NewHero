using SemihOrhan.WaveOne;
using System.Collections;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    [SerializeField] private TMPro.TMP_Text tutorialText;
    [SerializeField] private string[] tutorialContent;
    [SerializeField] private Lean.Gui.LeanWindow stageTransitionPanel;
    [SerializeField] private Transform heroUI;
    [SerializeField] private GameObject player;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private GameObject obstacleTutorial;
    [SerializeField] private Transform heroSpawnPosition;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private ProgressStageTrigger progressStageTrigger;

    private ushort m_TutorialIndex;

    // Start is called before the first frame update
    void Start()
    {
        tutorialText.text = tutorialContent[m_TutorialIndex];
        GetUIInfo(player);
        cameraController.CameraSetup();
        StartCoroutine(CloseTransitionPanel());
        progressStageTrigger.OpenDoor();
    }

    private void GetUIInfo(GameObject _player)
    {
        CharacterData heroData = _player.GetComponent<CharacterData>();
        heroData.healthBar = heroUI.GetChild(0).GetComponent<HealthBarGO>();
        heroData.healthBar.PosSetting(player);
        heroData.healthBar.distance = 3.5f;
        heroData.energyBar = heroUI.GetChild(1).GetComponent<HealthBarGO>();
        heroData.energyBar.PosSetting(player);
        heroData.energyBar.distance = 2.9f;
    }

    public void NextStage()
    {
        progressStageTrigger.CloseDoor();
        player.transform.position = heroSpawnPosition.position;
        m_TutorialIndex++;
        if (m_TutorialIndex < 3)
        {
            tutorialText.text = tutorialContent[m_TutorialIndex];
        }
        switch (m_TutorialIndex)
        {
            case 2:
                StartCoroutine(CloseTransitionPanel());
                obstacleTutorial.SetActive(true);
                waveManager.StartAllConfigWaves(m_TutorialIndex - 2);
                break;
            case 3:
                GameFlowManager.Instance.ChooseMap(0);
                sceneLoader.LoadMapUsingAddessable(1);
                break;
            default:
                StartCoroutine(CloseTransitionPanel());
                waveManager.StartAllConfigWaves(m_TutorialIndex - 2);
                break;
        }
    }

    private IEnumerator CloseTransitionPanel()
    {
        yield return new WaitForSeconds(0.5f);
        stageTransitionPanel.TurnOff();
    }
}
