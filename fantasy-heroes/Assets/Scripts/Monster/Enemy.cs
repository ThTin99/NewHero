using UnityEngine;

public enum NPC_EnemyState { IDLE, INSPECT, ATTACK, DEAD, NONE }

public enum NPC_WeaponType { MELEE, RANGE, MULTI_RANGE, RANDOM_RANGE , BUTT, BUTT_RANGE, TWIST, SPIT, SUMMONER}

public class Enemy : MonoBehaviour, IDestroyable
{
    [HideInInspector] public CharacterData m_CharacterData;
    [HideInInspector] public GameObject player;
    [HideInInspector] public Character _player;
    [HideInInspector] public bool isDie;
    public NPC_WeaponType weaponType = NPC_WeaponType.RANGE;
    public NPC_EnemyState idleState = NPC_EnemyState.IDLE;
    public NPC_EnemyState currentState = NPC_EnemyState.NONE;
    public EnemyAttackController enemyAttackController;
    public Animator npcAnimator;
    public Transform mindPoint, weaponPivot;
    public float weaponRange, scaleValue;
    Vector3 targetPos, startingPos;
    delegate void InitState();
    delegate void UpdateState();
    delegate void EndState();
    InitState _initState;
    InitState _updateState;
    InitState _endState;
    bool actionDone;
    Collider m_Collider;
    [SerializeField] ParticleSystem Enchant;
    [SerializeField] ParticleSystem Nova;
    private void OnEnable()
    {
        EventManager.StartListening(EventManager.GameEventEnum.ON_HERO_DIE, ResetTarget);
        EventManager.StartListening(EventManager.GameEventEnum.ON_REVIVE_HERO, OnHeroResurection);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventManager.GameEventEnum.ON_HERO_DIE, ResetTarget);
        EventManager.StopListening(EventManager.GameEventEnum.ON_REVIVE_HERO, OnHeroResurection);
    }

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        _player = player.GetComponent<Character>();
        m_Collider = GetComponent<Collider>();
        m_CharacterData = player.GetComponent<CharacterData>();
    }

    void Start()
    {
        if(gameObject.layer == LayerMask.NameToLayer("EnemyFly"))
        transform.position = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
        
        transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
        isDie = false;
        startingPos = transform.position;
        VfxManager.Instance.PlayVFX(VFX_TYPE.RESURECTION ,transform.position);
    }

    public void AttackAction()
    {
        switch (weaponType)
        {
            case NPC_WeaponType.MELEE:
                enemyAttackController.MeleeAttack();
                break;
            case NPC_WeaponType.RANGE:
                enemyAttackController.RangedAttack();
                break;
            case NPC_WeaponType.TWIST:
                enemyAttackController.TwistAttack();
                break; 
            case NPC_WeaponType.SPIT:
                enemyAttackController.Spit();
                break;
            case NPC_WeaponType.MULTI_RANGE:
                enemyAttackController.MultiRangeAttack();
                break;    
            case NPC_WeaponType.RANDOM_RANGE:
                enemyAttackController.RandomRangedAttack();
                break;
            case NPC_WeaponType.SUMMONER:
                enemyAttackController.SummonAttack();
                break;     
        }
    }

     void EndAttack()
    {
        SetState(NPC_EnemyState.INSPECT);
    }

    internal bool CheckDistanceToAttack()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= weaponRange)
        {
            return true;
        }
        return false;
    }

    internal void IdleAnimation()
    {
        npcAnimator.SetFloat("Speed", 0f);
    }

    internal void MoveAnimation()
    {
        npcAnimator.SetFloat("Speed", 1f);
    }

    internal void AttackAnimation()
    {
        npcAnimator.SetBool("Attack", true);
        switch (weaponType)
        {
            case NPC_WeaponType.SPIT:
                SpitAttackAnimation();
                break;
            case NPC_WeaponType.MULTI_RANGE:
                MultiRangeAttackAnimation();
                break;          
        }
        RotateAttack();
    }

    internal void SpitAttackAnimation(){
        int rand = Random.Range(0,2);
        bool isRotateLeft = false; 
        if(rand == 0) isRotateLeft = true;
        npcAnimator.SetBool("isLeftAttack", isRotateLeft);
    }

    internal void MultiRangeAttackAnimation(){
        string[] _stringList = new string[] {"isCenterAttack", "isRightAttack" , "isLeftAttack"};
        int rand = Random.Range(0,_stringList.Length);
        for(int i=0; i< _stringList.Length; i++ ){
            npcAnimator.SetBool(_stringList[i], false);
        }
        npcAnimator.SetBool(_stringList[rand], true);
    }

    public void EndAtk()
    {
        npcAnimator.SetBool("Attack", false);
        if(weaponType == NPC_WeaponType.SPIT){
            enemyAttackController.StopSpit();
        }
    }

    public void SetTargetPos(Vector3 newPos)
    {
        if (isDie || noTarget) return;
        targetPos = newPos;
        if (currentState != NPC_EnemyState.ATTACK)
        {
            SetState(NPC_EnemyState.INSPECT);
        }
    }

    private bool noTarget = false;

    public void ResetTarget()
    {
        SetState(idleState);
        noTarget = true;
    }

    public void OnHeroResurection()
    {
        noTarget = false;
    }

    bool IsClearMap()
    {
        foreach (Enemy e in GameManager.Instance.enemyList)
        {
            if (e.isDie == false) return false;
        }
        return true;
    }

    private void HealPlayer()
    {
        CharacterData characterData = _player.characterData;
        int healTriggerRate = 100 - characterData.CurrentPercentHP;
        if (Random.Range(0, 100f) <= healTriggerRate && healTriggerRate > 50)
        {
            VfxManager.Instance.PlayVFX(VFX_TYPE.HEAL ,_player.transform.position);
            //Minus dmg will heal instead
            characterData.Stats.Damage(-(Random.Range(8, 10) * characterData.Stats.CurrentMaxHealth / 100), false, true);
            characterData.UpdateHealthBar();
        }
    }

    public void Die()
    {
        isDie = true;
        m_Collider.enabled = false;
        Destroy(_player.currentTargetLive);
        SetState(NPC_EnemyState.DEAD);
        npcAnimator.SetTrigger("Dead");
        HealPlayer();
        _player.SetState(CharacterState.IDLE);
        EventManager.TriggerEvent(EventManager.GameEventEnum.ON_ENEMY_DIE);
        if (IsClearMap()) ProgressStageTrigger.Instance.OpenDoor();
    }

    public void DestroyEnemy()
    {
        VfxManager.Instance.PlayVFX(VFX_TYPE.DESTROY ,transform.position);
        Destroy(gameObject);
    }

    public void CloseEnchant(){
        if(Enchant != null){
            Enchant.gameObject.SetActive(false);
            Enchant.Stop();
        }

        if(Nova != null){
            Nova.gameObject.SetActive(true);
            Nova.Play();
        }
        Invoke("CloseNova", 0.4f);
    }

    void CloseNova(){
        if(Nova != null){
            Nova.gameObject.SetActive(false);
            Nova.Stop();
        }
    }

    internal void ShowEnchant(){
        if(Enchant != null){
            Enchant.gameObject.SetActive(true);
            Enchant.Play();
        }
    }

    void RotateAttack(){
        transform.LookAt(_player.mindPoint.position);
        weaponPivot.LookAt(_player.mindPoint.position);
        Vector3 temp = transform.rotation.eulerAngles;
        temp.x = 0;
        temp.z = 0;
        Quaternion rotation = Quaternion.Euler(temp);
        transform.rotation = rotation;
    }

    void StateUpdate_Death() { }
    void StateEnd_Death() { }
    void StateUpdate_Idle() { }
    void StateEnd_Idle() { }

    public void SetState(NPC_EnemyState newState)
    {
        if (currentState != newState)
        {
            _endState?.Invoke();
            switch (newState)
            {
                case NPC_EnemyState.IDLE: _initState = StateInit_Idle; _updateState = StateUpdate_Idle; _endState = StateEnd_Idle; break;
                case NPC_EnemyState.INSPECT: _initState = StateInit_Inspect; _updateState = StateUpdate_Inspect; _endState = StateEnd_Inspect; break;
                case NPC_EnemyState.ATTACK: _initState = StateInit_Attack; _updateState = StateUpdate_Attack; _endState = StateEnd_Attack; break;
                case NPC_EnemyState.DEAD: _initState = StateInit_Death; _updateState = StateUpdate_Death; _endState = StateEnd_Death; break;
            }
            _initState();
            currentState = newState;
        }
    }
    void StateInit_Idle()
    {

    }

    private void StateInit_Death()
    {
        CoinManager.Instance.SpawnSomeCoinsAtPosition(transform.position);
        EventManager.TriggerEvent(EventManager.GameEventEnum.KILL_MONSTER, 1);
    }

    void StateInit_Inspect()
    {

    }

    void StateUpdate_Inspect()
    {
        npcAnimator.SetFloat("Speed", 1f);
        if (CheckDistanceToAttack())
        {
            npcAnimator.SetFloat("Speed", 0f);
            SetState(NPC_EnemyState.ATTACK);
        }
    }

    void StateEnd_Inspect()
    {

    }

    void StateInit_Attack()
    {
        npcAnimator.SetBool("Attack", true);
        CancelInvoke(nameof(AttackAction));
        actionDone = false;
        RotateAttack();
    }

    void StateUpdate_Attack()
    {
        
    }

    void StateEnd_Attack()
    {
        SetTargetPos(player.transform.position);
        npcAnimator.SetBool("Attack", false);
    }
}