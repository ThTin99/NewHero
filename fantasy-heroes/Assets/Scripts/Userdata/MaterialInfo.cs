[System.Serializable]
public class MaterialInfo
{
    public MaterialType _type;
    public int _total;

    public MaterialInfo(MaterialType type, int number)
    {
        _type = type;
        _total = number;
    }
}