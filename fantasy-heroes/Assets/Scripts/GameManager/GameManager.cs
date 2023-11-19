using SemihOrhan.WaveOne;
using SemihOrhan.WaveOne.StartPoints;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [Header("Monster Wave")]
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private MonsterWaveScript[] monsterWaves;
    MonsterWaveScript curentMonsterWaves;

    [Header("Obstacles")]
    [SerializeField] private ObtaclesScript previousObstacle;
    [SerializeField] private MapData mapData;
    [SerializeField] private Transform[] spawnPos;

    [Header("Reward Turret")]
    [SerializeField] private GameObject rewardTurret;
    [SerializeField] private GameObject rewardPixie;
    [SerializeField] private Vector3 spawmPosition;

    [Header("Player & map")]
    [SerializeField] private Character[] playerList;
    [SerializeField] private MapScript mapAsset;
    MapScript curentMap;

    [Header("Reward Turret")]
    [SerializeField] private MovementJoystick movementJoystick;

    private Character player;
    private ProgressStageTrigger nextStageDoor;
    private Vector3 playerStartPos;
    private bool canChooseAbility;
    private Text m_StageText;
    [SerializeField] private Transform HeroUI;

    [Header("Enemy List")]
    public List<Enemy> enemyList = new List<Enemy>();
    internal int currentStage = 0;

    public Transform bulletHolder, monsterHolder, effectHolder;
    public FantasyHero.SoundManager soundManager;

    [Header("Ability Description pop position")]
    public RectTransform abilityPopupPos;
    int countComplete = 0;


    private void OnEnable()
    {
        EventManager.StartListening(EventManager.GameEventEnum.ON_BOSS_DEFEATED, SpawmRewardTurretAfterDefeatingABoss);
        
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventManager.GameEventEnum.ON_BOSS_DEFEATED, SpawmRewardTurretAfterDefeatingABoss);
    }

    void Start()
    {
        // if (GameFlowManager.Instance.UserProfile.IsClearedTutorials == false)
        // {
        //     return;
        // }
        SetupMapData();
        EventManager.TriggerEvent(EventManager.GameEventEnum.PLAY_GAME, UnitQuest.QuestType.PLAY_GAMES, 1);
    }

    private void SetupMapData()
    {
        MapDatabase mapDatabase = GameFlowManager.Instance.GetMapDatabase();
        monsterWaves = mapDatabase.monsterWaves;
        mapAsset = mapDatabase.mapAsset;
        mapData = mapDatabase.obstacleData;
        playerStartPos = mapDatabase.playerSpawmPosition;
        curentMap = Instantiate(mapAsset);
        player = Instantiate(playerList[(int)GameFlowManager.Instance.UserProfile.selectedHero], playerStartPos, Quaternion.identity);
        curentMonsterWaves = Instantiate(monsterWaves[currentStage]);
    }

    public void SetupMapComplete(){
        countComplete++;
        if(countComplete >= 3){
            PlayerSetup();
            NextStage();
        }
    }

    void PlayerSetup(){
        GetUIInfo(player);
        FindObjectOfType<CameraController>().CameraSetup();
        FindObjectOfType<CoinManager>().CoinManagerSetup();
        nextStageDoor = FindObjectOfType<ProgressStageTrigger>();
    }

    private void GetUIInfo(Character _player)
    {
        CharacterData heroData = _player.GetComponent<CharacterData>();
        heroData.healthBar = HeroUI.GetChild(0).GetComponent<HealthBarGO>();
        heroData.healthBar.PosSetting(_player.gameObject);
        heroData.healthBar.distance = 3.5f;
        heroData.energyBar = HeroUI.GetChild(1).GetComponent<HealthBarGO>();
        heroData.energyBar.PosSetting(_player.gameObject);
        heroData.energyBar.distance = 2.9f;
        heroData.UpdateMaxHealth();
    }

    private void TurnOffTransitionVfx()
    {
        EventManager.TriggerEvent(EventManager.GameEventEnum.ON_LOAD_COMPLETED);
    }

    internal void NextStage()
    {
        if (currentStage >= 25)
        {
            EventManager.TriggerEvent(EventManager.GameEventEnum.ON_CLEARED_MAP);
            TurnOffTransitionVfx();
            GameFlowManager.Instance.SetChapterOpen();
            return;
        }
        
        ClearAllObjectInHolders();
        ProcessNextStage();
    }

    public void ProcessNextStage()
    {
        //Reset joystick interact when change stage
        movementJoystick.PointerUp();
        player.transform.position = playerStartPos;
        CameraController.Instance.SnapCamera(true);
        CloseDoor();

        if ((currentStage + 1) % 3 == 0 && currentStage != 0)
        {
            canChooseAbility = true;
        }

        if ((currentStage + 1) == 5 || (currentStage + 1) == 15)
        {
            SpawmRewardTurretAfterDefeatingABoss();
        }

        if (canChooseAbility)
        {
            ShowAbilityChooseCanvasAndPauseGame();
        }

        CreateOstacle();
    }

    internal void CleanObstacle()
    {
        previousObstacle.Destroy();
    }

    internal void CleanGameObject()
    {
        foreach (Transform child in bulletHolder)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in monsterHolder)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in effectHolder)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreateOstacle()
    {
        previousObstacle = Instantiate(mapData.maps[0].obstacles[currentStage]);
        //m_StageText = GameObject.FindGameObjectWithTag("Stage Text").GetComponent<Text>();
        //m_StageText.text = currentStage.ToString();
    }

    public void CreateObstaclesComplete(){
        if (waveManager.WaveConfigurators.Count == 0)
        {
            waveManager.Findconfigs();
        }

        UpdateStartSpawmPoint();

        currentStage++;

        curentMap.SetStage(currentStage.ToString()); 

        StartingAllConfigWaves();

        TurnOffTransitionVfx();
    }

    private void UpdateStartSpawmPoint()
    {
        var startPoints = waveManager.WaveConfigurators[0].gameObject.GetComponent<ListOfTransforms>().StartPoints;
        startPoints.Clear();
        for (int i = 0; i < mapData.maps[0].spawnPositions[currentStage].positions.Length; i++)
        {
            spawnPos[i].position = mapData.maps[0].spawnPositions[currentStage].positions[i];
            startPoints.Add(spawnPos[i]);
        }
    }

    void StartingAllConfigWaves()
    {
        waveManager.StartAllConfigWaves(currentStage - 1);
    }

    public int GetEnemyCount()
    {
        return enemyList.Count;
    }

    public void AddEnemyToList(GameObject obj)
    {
        enemyList.Add(obj.GetComponent<Enemy>());
    }

    internal void ShowAbilityChooseCanvasAndPauseGame()
    {
        UIManager.Instance.ShowAbilityPanel();
        TimeManager.Instance.IgnoreUpdate = true;
        Time.timeScale = 0;
    }

    internal void ShowLuckyWheelCanvas()
    {
        UIManager.Instance.ShowLuckyWheelPanel();
    }

    internal void CloseAbilityChooseCanvasAndResumeTimeThenSetFlagCantChooseAbility()
    {
        UIManager.Instance.CloseAbilityPanel();
        Time.timeScale = 1.0f;
        TimeManager.Instance.IgnoreUpdate = false;
        canChooseAbility = false;
    }

    internal void CloseLuckyWheelCanvasThenSetFlagCantRollLuckWheel()
    {
        UIManager.Instance.CloseLuckyWheelPanel();
    }

    internal void ClearAllObjectInHolders()
    {
        foreach (Transform bullet in bulletHolder)
        {
            Destroy(bullet.gameObject);
        }
    }

    internal void CloseDoor()
    {
        nextStageDoor.CloseDoor();
    }

    internal void SpawmRewardTurretAt(Vector3 position)
    {
        Instantiate(rewardTurret, position, Quaternion.identity, monsterHolder);
        Instantiate(rewardPixie, position + new Vector3(0, 0, -15f), Quaternion.identity, monsterHolder);
    }

    public void SpawmRewardTurretAfterDefeatingABoss()
    {
        SpawmRewardTurretAt(spawmPosition);
    }

    //Use SoundManager or attach to gameobject and call via LeanPlaySound
    public void PlayUISound(string soundName)
    {
        SoundManager.PlaySFX(soundName);
    }

    public void PlayUISound(AudioClip audioClip)
    {
        SoundManager.PlaySFX(audioClip);
    }
}
