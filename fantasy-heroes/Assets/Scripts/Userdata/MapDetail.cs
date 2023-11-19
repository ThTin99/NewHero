public partial struct ProcessMapData
{
    [System.Serializable]
    public struct MapDetail
    {
        public CampaignMode difficulty;//currently unlocked difficult
        public uint clearedstage;//number of stage cleared

        public MapDetail(CampaignMode difficulty, uint clearedstage)
        {
            this.difficulty = difficulty;
            this.clearedstage = clearedstage;
        }
    }
}