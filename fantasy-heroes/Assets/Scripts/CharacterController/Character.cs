using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CharacterState { IDLE, MOVE, ATTACK, DEAD }

public class Character : MonoBehaviour, IDestroyable, ICanRollLuckyWheel, IRepaireable, IResurrectionable
{
    private const string ATTACK_SPEED_HASH = "AttackSpeed";

    public CharacterState characterState;
    public CharacterData characterData;
    internal Rigidbody rb;
    internal Animator anim;
    internal FXController fXController;
    public bool isHavePet;
    public Pet pet;
    public float scaleValue, xRotateWhenAttack, energyValue;
    public Enemy targetEnemy;
    public GameObject targetEnemyIconPrefab;
    public GameObject currentTargetLive;
    public Transform mindPoint, weaponPivot;
    internal AttackController attackController;
    [SerializeField] ParticleSystem Enchant;   

    public int projectileLevel = 1;
    public bool angleShot;
    public bool sideShot;
    public bool barrage;
    private bool torrentialPower;
    private bool fullCharge;
    private bool dealthBlow;
    private bool targetWeakPoint;
    private bool metalStorm;
    private bool tsarBomba;
    private bool dodge;
    public bool splitShot;
    private bool unstableTech;
    private bool secondWind;
    private bool fireShot;          //25% strength for 8 s
    private bool coldShot;          //apply slow effect 50%
    private bool electricShot;      //25% strength
    private bool corrodeShot;       //70% strength until death
    private float baseAttackSpeed = 1;
    [SerializeField] Cloth Cloth;

    List<Enemy> _listEnemy = new List<Enemy>();
    private void OnEnable()
    {
        EventManager.StartListening(EventManager.GameEventEnum.ON_ENEMY_DIE, FindTarget);
        EventManager.StartListening(EventManager.GameEventEnum.ON_JOYSTICK_IDLE, FindTarget);
        EventManager.StartListening(EventManager.GameEventEnum.ON_REVIVE_HERO, ResurrectionBackToBattle);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventManager.GameEventEnum.ON_ENEMY_DIE, FindTarget);
        EventManager.StopListening(EventManager.GameEventEnum.ON_JOYSTICK_IDLE, FindTarget);
        EventManager.StopListening(EventManager.GameEventEnum.ON_REVIVE_HERO, ResurrectionBackToBattle);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        characterData = GetComponent<CharacterData>();
        fXController = GetComponent<FXController>();
        attackController = GetComponent<AttackController>();
        fXController.GetPos();
        if(Cloth != null){
            Cloth.enabled = false;
            Invoke("EnableCloth", 0.5f);
        }
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);
        yield return new WaitUntil(() => VfxManager.Instance != null);
        GameManager.Instance.SetupMapComplete();
        SetState(CharacterState.IDLE);
        transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
        //StartValueSetting();
        if (isHavePet)
        {
            Pet _pet = Instantiate(pet, transform);
            _pet.PossitionSetting(transform.position);
            _pet.Hero = this;
        }
        if (characterData.characterType == CharacterType.HERO)
        {
            fXController.PlayFX(FXState.APPEAR);
        }

        _listEnemy = GameManager.Instance.enemyList;

        SetBackCollisionWithObstacle();
    }

    private void Update()
    {
        TargetMonsterIconManager();
    }

    void EnableCloth(){
        Cloth.enabled = true;
    }

    void TargetMonsterIconManager()
    {
        if (targetEnemy != null && !targetEnemy.isDie && currentTargetLive == null)
        {
            currentTargetLive = Instantiate(targetEnemyIconPrefab, targetEnemy.transform);
            currentTargetLive.transform.GetChild(0).transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        }

        if (targetEnemy != null && targetEnemy.isDie && currentTargetLive)
        {
            Destroy(currentTargetLive);
        }

        if (characterState == CharacterState.MOVE)
        {
            if (currentTargetLive)
            {
                Destroy(currentTargetLive);
            }
        }
    }

    

    private static void SetBackCollisionWithObstacle()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Obstacle"), LayerMask.NameToLayer("Player"), false);
    }

    internal void ChangeLayerToGoThroughWalls()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Obstacle"), LayerMask.NameToLayer("Player"));
    }

    internal void Cut20PercentAtkSpeedFor50PercentDamage()
    {
        baseAttackSpeed = 0.8f;
        var currentAttackSpeed = anim.GetFloat(ATTACK_SPEED_HASH) * baseAttackSpeed;
        anim.SetFloat(ATTACK_SPEED_HASH, currentAttackSpeed);
        tsarBomba = true;
    }
    public void MoveRotate(Vector3 pos)
    {
        float angle = Mathf.Atan2(pos.x, pos.y) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, new Vector3(0, 1, 0));
        transform.rotation = rotation;
        SetXRotate(false);
    }

    public void SetXRotate(bool isAttack)
    {
        Vector3 temp = transform.rotation.eulerAngles;
        int Direction = 1;
        if (isAttack)
        {
            if (temp.y > -90 && temp.y < 90) Direction = 1;
            else Direction = -1;
        }
        temp.x = xRotateWhenAttack * Direction;
        transform.rotation = Quaternion.Euler(temp);
    }

    void ResetAllAnimation()
    {
        anim.ResetTrigger("Idle");
        anim.ResetTrigger("Run");
        anim.ResetTrigger("Attack");
        anim.ResetTrigger("Attack");
    }

    public void FindTarget()
    {
        targetEnemy = FindNearestEnemy();
        if (targetEnemy)
        {
            RotateTowardEnemy();
            SetState(CharacterState.ATTACK);
        }
        else
        {
            SetState(CharacterState.IDLE);
            EventManager.TriggerEvent(EventManager.GameEventEnum.ON_CLEARED_STAGE);
        }
    }

    Enemy FindNearestEnemy()
    {
        List<Enemy> currentEnemyList = PureList();
        List<float> distanceList = new List<float>();
        for (int i = 0; i < currentEnemyList.Count; i++)
        {
            //if(currentEnemyList[i].GetComponent<Enemy>().isDie) continue;
            float distance = Vector3.Distance(transform.position, currentEnemyList[i].transform.position);
            distanceList.Add(distance);
        }
        if (distanceList.Count <= 0)
        {
            ProgressStageTrigger.Instance.OpenDoor();
            return null;
        }
        float min = distanceList.Min();
        int position = distanceList.IndexOf(min);
        return currentEnemyList[position];
    }

    List<Enemy> PureList()
    {
        for (int i = _listEnemy.Count - 1; i >= 0; i--)
        {
            if (_listEnemy[i] == null)
            {
                _listEnemy.RemoveAt(i);
            }
            else if (_listEnemy[i].isDie)
            {
                _listEnemy.RemoveAt(i);
            }
        }
        return _listEnemy;
    }


    public void RotateTowardEnemy()
    {
        if (targetEnemy)
        {
            LookAtSetting();
            SetXRotate(true);
        }
    }

    void LookAtSetting(){
        transform.LookAt(targetEnemy.mindPoint.position);
        Vector3 temp = transform.rotation.eulerAngles;
        temp.x = 0;
        temp.z = 0;
        Quaternion rotation = Quaternion.Euler(temp);
        transform.rotation = rotation;
    }

    public int ProjectileLevel { get => projectileLevel; set => projectileLevel = value > 3 ? 3 : value; }
    public bool AngleShot { get => angleShot; set => angleShot = value; }
    public bool SideShot { get => sideShot; set => sideShot = value; }
    public bool Barrage { get => barrage; set => barrage = value; }
    public bool TorrentialPower { get => torrentialPower; set => torrentialPower = value; }
    public bool FullCharge { get => fullCharge; set => fullCharge = value; }
    public bool DealthBlow { get => dealthBlow; set => dealthBlow = value; }
    public bool TargetWeakPoint { get => targetWeakPoint; set => targetWeakPoint = value; }
    public bool MetalStorm { get => metalStorm; set => metalStorm = value; }
    public bool TsarBomba { get => tsarBomba; set => tsarBomba = value; }
    public bool Dodge { get => dodge; set => dodge = value; }
    public bool SplitShot { get => splitShot; set => splitShot = value; }
    public bool UnstableTech { get => unstableTech; set => unstableTech = value; }
    public bool SecondWind { get => secondWind; set => secondWind = value; }
    public bool FireShot { get => fireShot; set => fireShot = value; }
    public bool ColdShot { get => coldShot; set => coldShot = value; }
    public bool ElectricShot { get => electricShot; set => electricShot = value; }
    public bool CorrodeShot { get => corrodeShot; set => corrodeShot = value; }

    public void SetState(CharacterState _characterState)
    {
        if (characterState == CharacterState.DEAD) return;
        if (this.characterState != _characterState)
        {
            switch (_characterState)
            {
                case CharacterState.IDLE:
                    Idle();
                    break;
                case CharacterState.MOVE:
                    Move();
                    break;
                case CharacterState.ATTACK:
                    Attack();
                    break;
                case CharacterState.DEAD:
                    this.characterState = CharacterState.DEAD;
                    targetEnemy = null;
                    fXController.PlayFX(FXState.DEAD);
                    SetAnimation("Dead");
                    EventManager.TriggerEvent(EventManager.GameEventEnum.ON_HERO_DIE);
                    break;
                default:
                    break;
            }
        }
    }

    void Attack()
    {
        if (characterState != CharacterState.DEAD)
        {
            this.characterState = CharacterState.ATTACK;
            if(!attackController.isPowerShot){
                SetAnimation("Attack");
            }else{
                SetAnimation("PowerAttack");
                ShowEnchant(true);
            }
        }
    }

    internal void ShowEnchant(bool isShow){
        if(Enchant != null){
            Enchant.gameObject.SetActive(isShow);
            if(isShow){
                Enchant.Play();
            }else{
                Enchant.Stop();
            }
        }
    }

    void Move()
    {
        if (characterState != CharacterState.DEAD)
        {
            this.characterState = CharacterState.MOVE;
            targetEnemy = null;
        }
    }

    public void MoveSmokeEffect()
    {
        fXController.PlayFX(FXState.MOVE);
    }

    void Idle()
    {
        if (characterState != CharacterState.DEAD)
        {
            this.characterState = CharacterState.IDLE;
            RotateTowardEnemy();
            if (TryGetComponent(out MovementController movementController))
            {
                movementController.Idle();
            }
        }
    }

    public void Die()
    {
        SetState(CharacterState.DEAD);
    }

    public void SetAnimation(string _anim)
    {
        ResetAllAnimation();
        anim.SetTrigger(_anim);
    }

    public void AttackUp(int percent)
    {
        characterData.Stats.CurrentStats.strength += characterData.Stats.CurrentStats.strength * percent / 100;
    }

    public void ShootFaster()
    {
        var currentAttackSpeed = anim.GetFloat(ATTACK_SPEED_HASH);
        currentAttackSpeed += baseAttackSpeed * 40 / 100;
        currentAttackSpeed = currentAttackSpeed > 3 ? 3 : currentAttackSpeed;//limit attack speed to x3 time
        anim.SetFloat(ATTACK_SPEED_HASH, currentAttackSpeed);
    }

    public void ShowLuckyWheel()
    {
        UIManager.Instance.ShowLuckyWheelPanel();
    }

    public void Repairing()
    {
        UIManager.Instance.ShowWorkShopPanel();
    }

    public void ResurrectionBackToBattle()
    {
        characterState = CharacterState.IDLE;
        SetState(CharacterState.IDLE);
        characterData.ResurrectionBackToBattle();
    }
}
