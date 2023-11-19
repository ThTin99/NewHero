using System.Collections.Generic;

public class EquipmentDatabaseManager : Singleton<EquipmentDatabaseManager>
{
    public List<Item> AllItems;
    public List<EnhanceRequirement> EnhanceRequirements;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
}

[System.Serializable]
public struct EnhanceRequirement
{
    [System.Serializable]
    public struct RequiredMaterial
    {
        public MaterialItem MaterialItem;
        public int Amount;
    }

    public UnitRank Rank;
    public List<RequiredMaterial> RequiredMaterials;
}