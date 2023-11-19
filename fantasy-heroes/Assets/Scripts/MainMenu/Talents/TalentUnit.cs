using UnityEngine;
using static StatSystem;

public enum TalentType { HealthUp, MoveSpeedUp, RapidFire, LevelUpHpRecoveryUp, ReinforcedArmor, RepairKit, DestructiveForce, CritDmgUp, CritRateUp }

[System.Serializable]
public struct TalentUnit
{
    public string Name;
    public TalentType TalentType;
    public Sprite TalentIcon;
    public string Description;
    public StatModifier StatModifier;
}
