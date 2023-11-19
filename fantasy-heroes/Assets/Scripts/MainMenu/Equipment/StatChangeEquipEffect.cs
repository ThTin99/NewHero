public class StatChangeEquipEffect : EquipmentItem.EquippedEffect
{
    public StatSystem.StatModifier Modifier;

    public override void Equipped(CharacterData user)
    {
        user.Stats.AddModifier(Modifier);
    }

    public override void Removed(CharacterData user)
    {
        user.Stats.RemoveModifier(Modifier);
    }

    public override string GetDescription()
    {
        string unit = Modifier.ModifierMode == StatSystem.StatModifier.Mode.Percentage ? "%" : "";

        var desc = $"{Modifier.Stats.CurrentStrength:0;-#}{unit}\n"; //format specifier to force the + sign to appear
        desc += $"{Modifier.Stats.CurrentHealth:0;-#}{unit}\n";
        desc += $"{Modifier.Stats.CurrentHealth:0;-#}{unit}\n";
        desc += $"{Modifier.Stats.agility:0;-#}{unit}\n";
        desc += $"{Modifier.Stats.CurrentCrit:0;-#}{unit}\n";

        return desc;
    }
}
