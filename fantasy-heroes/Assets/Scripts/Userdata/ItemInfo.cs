[System.Serializable]
public class ItemInfo
{
    public ItemType _type;
    public UnitRank _rank;
    public int _total;

    public ItemInfo(ItemType type, UnitRank rank, int total)
    {
        _type = type;
        _rank = rank;
        _total = total;
    }
}