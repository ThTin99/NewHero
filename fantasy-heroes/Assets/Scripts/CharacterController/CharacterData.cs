using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum CharacterType { HERO, MONSTER, BOSS, PET }

public class CharacterData : MonoBehaviour, IDamageable, ISlowable, ICanReceiveElementalEffect
{
    public CharacterType characterType;
    public string CharacterName;

    public StatSystem Stats;
    /// <summary>
    /// The starting weapon equipped when the Character is created. Set through the Unity Editor.
    /// </summary>
    public Weapon StartingWeapon;
    public EquipmentSystem Equipment = new EquipmentSystem();
    public Action OnDamage { get; set; }
    public HealthBarGO healthBar;
    public HealthBarGO energyBar;
    ChangeColorMaterial changeColorMaterial;
    Character character;
    public bool isDie = false;
    public bool Initiated;

    private NavMeshAgent m_Agent;

    private void Awake()
    {
        changeColorMaterial = GetComponent<ChangeColorMaterial>();
    }

    private void Start()
    {
        Init();
        TryGetComponent(out character);
    }

    public void Init()
    {
        Stats.Init(this);

        //Talent stats handler
        if (characterType == CharacterType.HERO)
        {
            // GameFlowManager instance = GameFlowManager.Instance;
            // UserProfile userProfile = instance.UserProfile;
            // System.Collections.Generic.Dictionary<TalentType, ushort> talentLevelList = userProfile.TalentLevelList;
            // for (int i = 0; i < talentLevelList.Count; i++)
            // {
            //     ushort talentLevel = talentLevelList[instance.TalentDatabase.TalentUnits[i].TalentType];
            //     if (talentLevel > 0)
            //     {
            //         StatSystem.StatModifier statModifier = instance.TalentDatabase.TalentUnits[i].StatModifier;
            //         statModifier.Stats.agility *= (int)Mathf.Pow(1.1f, talentLevel);
            //         statModifier.Stats.health *= (int)Mathf.Pow(1.5f, talentLevel);
            //         statModifier.Stats.attackSpeedBoost *= (short)Mathf.Pow(1.5f, talentLevel);
            //         statModifier.Stats.defense *= (short)Mathf.Pow(1.1f, talentLevel);
            //         statModifier.Stats.healingEffectiveness *= (short)Mathf.Pow(1.5f, talentLevel);
            //         statModifier.Stats.strength *= (short)Mathf.Pow(1.1f, talentLevel);
            //         statModifier.Stats.crit *= (short)Mathf.Pow(1.1f, talentLevel);
            //         Stats.AddModifier(statModifier);
            //     };
            // }
        }

        if (StartingWeapon != null)
        {
            Equipment.InitWeapon(StartingWeapon, this);
        }
        Equipment.Init(this, characterType == CharacterType.HERO);

        if (characterType != CharacterType.HERO)
        {
            HealhBarUI.Instance.CreateNewHealthBar(gameObject);
        }

        if (characterType == CharacterType.BOSS)
        {
            EventManager.TriggerEvent(EventManager.GameEventEnum.ON_BOSS_APPEARING);
        }

        TryGetComponent(out m_Agent);
        TryGetComponent(out changeColorMaterial);

        Initiated = true;
    }

    public void UpdateMaxHealth(){
        healthBar.HealthSetting(Stats.CurrentMaxHealth, Stats.CurrentMaxHealth);
    }

    private void Update()
    {
        Stats.Tick();
    }

    public void Death()
    {
        isDie = true;
        switch (characterType)
        {
            case CharacterType.HERO when character.SecondWind:
                character.SecondWind = false;//immediately resurect him and remove seconwind ability if you're a Hero!
                ResurrectionBackToBattle();
                return;
            case CharacterType.BOSS:
                EventManager.TriggerEvent(EventManager.GameEventEnum.ON_BOSS_DEFEATED);
                break;
        }

        Stats.Death();
        changeColorMaterial.ChangeColorToDie();
        if (TryGetComponent(out IDestroyable destroyable))
        {
            destroyable.Die();
        }

        if(characterType != CharacterType.HERO){
            healthBar.gameObject.SetActive(false);
            HealhBarUI.Instance.RetrieveHealthBar(healthBar.gameObject);
        }
    }

    [Range(1, int.MaxValue)]
    private int finalDamage;

    public void Damage(int attackData, bool dealthBlow = false, bool crit = false, bool dealMoreDamageBasedOnHp = false)
    {
        switch (characterType)
        {
            case CharacterType.HERO when character.Dodge && Random.Range(0, 100.0f) <= 20.0f:
                DamageUI.Instance.NewDamage(0, transform.position, true, "Dodge");
                return;
            case CharacterType.MONSTER when dealthBlow:
                Stats.Damage(Stats.CurrentMaxHealth, true);
                break;
            default:
                finalDamage = crit && Check80PercentHp() ? attackData * 2 : attackData;

                if (dealMoreDamageBasedOnHp)//deal more damage with 1% current HP
                {
                    finalDamage += Stats.CurrentHealth / 100;
                }

                finalDamage -= Stats.CurrentMaxDefense;

                //randomize dmg output
                finalDamage = Random.Range(finalDamage - 2, finalDamage + 3);

                //no minus dmg
                finalDamage = Mathf.Clamp(finalDamage, 1, int.MaxValue);

                Stats.Damage(finalDamage);
                break;
        }

        OnDamage?.Invoke();

        if (characterType == CharacterType.HERO) character.fXController.PlayFX(FXState.GETDAMAGE);
        if (Stats.CurrentHealth <= 0)
            Death();
       
        changeColorMaterial.ChangeColorToDamaged();

        UpdateHealthBar();
    }

    internal void UpdateHealthBar()
    {
        healthBar.HealthSetting(Stats.CurrentMaxHealth, Stats.CurrentHealth);
        if (characterType == CharacterType.BOSS)
        {
            EventManager.TriggerEvent(EventManager.GameEventEnum.ON_BOSS_HEALTH_CHANGE, Stats.CurrentHealth * 100 / Stats.CurrentMaxHealth);
        }
    }

    private bool Check80PercentHp()
    {
        return Stats.CurrentHealth >= Stats.CurrentMaxHealth * 80 / 100;
    }

    public void ResurrectionBackToBattle()
    {
        isDie = false;
        Stats.ChangeHealth(Stats.CurrentMaxHealth);
        VfxManager.Instance.PlayVFX(VFX_TYPE.RESURECTION, transform.position);
        UpdateHealthBar();
    }

    public void ChangeAgentSpeed(int speed)
    {
        try
        {
            m_Agent.speed = speed;
        }
        catch (Exception)
        { }
    }

    public void ApplySlowEffect(int slowPercent, float duration)
    {
        StatSystem.StatModifier modifier = new StatSystem.StatModifier
        {
            ModifierMode = StatSystem.StatModifier.Mode.Percentage
        };
        modifier.Stats.agility = -slowPercent;

        //VFXManager.PlayVFX(VFXType.Stronger, user.transform.position);
        Stats.AddTimedModifier(modifier, duration, "Slow"/*, EffectSprite*/);
    }

    public void ApplyEffect(ElementalEffect elementalEffect)
    {
        Stats.AddElementalEffect(elementalEffect);
    }

    public int CurrentPercentHP => Stats.CurrentHealth * 100 / Stats.CurrentMaxHealth;
}
