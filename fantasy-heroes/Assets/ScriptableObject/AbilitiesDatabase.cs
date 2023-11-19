using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New AbilitiesDatabase")]
public class AbilitiesDatabase : ScriptableObject
{
    //for loot box, lucky wheel
    public List<Ability> abilities;

    //for heal or power turret
    public List<Ability> workshopAbility;
}

[System.Serializable]
public struct Ability
{
    public string abilityName;
    public string abilityDescription;
    public Sprite abilityIcon;
    public AbilityType abilityType;

    internal void ApplyEffect()
    {
        Character character = Object.FindObjectOfType<Character>();

        switch (abilityType)
        {
            case AbilityType.FRONT_SHOT:
                character.ProjectileLevel++;
                break;
            case AbilityType.ANGLE_SHOT:
                character.AngleShot = true;
                break;
            case AbilityType.SIDE_SHOT:
                character.SideShot = true;
                break;
            case AbilityType.BARRANGE:
                character.Barrage = true;
                break;
            case AbilityType.Torrential_power:
                character.TorrentialPower = true;
                break;
            case AbilityType.ATK_UP:
                character.AttackUp(30);
                break;
            case AbilityType.Full_charge:
                character.FullCharge = true;
                break;
            case AbilityType.ATK_SPEED:
                character.ShootFaster();
                break;
            case AbilityType.Deathblow:
                character.DealthBlow = true;
                break;
            case AbilityType.TargetWeakPoint:
                character.TargetWeakPoint = true;
                break;
            case AbilityType.MetalStorm:
                character.MetalStorm = true;
                break;
            case AbilityType.TsarBomba:
                character.Cut20PercentAtkSpeedFor50PercentDamage();
                break;
            case AbilityType.Dodge:
                character.Dodge = true;
                break;
            case AbilityType.SplitShot:
                character.SplitShot = true;
                break;
            case AbilityType.NoGuard:
                character.ChangeLayerToGoThroughWalls();
                break;
            case AbilityType.UnstableTech:
                character.UnstableTech = true;
                break;
            case AbilityType.SecondWind:
                character.SecondWind = true;
                break;
            case AbilityType.HpRecovery:
                CharacterData characterData = character.characterData;
                characterData.Stats.ChangeHealth(characterData.Stats.baseStats.health * 20 / 100);//heal 20% max Hp
                characterData.UpdateHealthBar();//update visual since function above doesn't do that
                break;
            case AbilityType.HpMaxUp:
                CharacterData characterData1 = character.characterData;
                var changeAmount = characterData1.Stats.baseStats.health * 20 / 100;
                characterData1.Stats.CurrentStats.health += changeAmount;
                characterData1.Stats.ChangeHealth(changeAmount);//heal current hp base on the increase amount of max Hp
                characterData1.UpdateHealthBar();//update visual since function above doesn't do that
                break;
            case AbilityType.IceBall:
                AbilitiesManager.Instance.SpawnIceBall();
                break;
            case AbilityType.ATK_UP_SMALL:
                character.AttackUp(10);
                break;
            case AbilityType.FIRE_SHOT:
                character.FireShot = true;
                break;
            case AbilityType.ELECTRIC_SHOT:
                character.ElectricShot = true;
                break;
            case AbilityType.Corrode_shot:
                character.CorrodeShot = true;
                break;
            case AbilityType.Cold_shot:
                character.ColdShot = true;
                break;
            default:
                break;
        }
    }
}
