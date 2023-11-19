[System.Serializable]
public partial struct ProcessMapData
{
    public int mapIndex; 
    public MapDetail[] mapDetails;

    public ProcessMapData(int mapIndex, MapDetail[] mapDetails)
    {
        this.mapIndex = mapIndex;
        this.mapDetails = mapDetails;
    }
}