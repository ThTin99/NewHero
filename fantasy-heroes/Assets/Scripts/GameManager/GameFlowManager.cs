using BayatGames.SaveGameFree;
using System.Threading.Tasks;
using UnityEngine;

public class GameFlowManager : Singleton<GameFlowManager>
{
    private const string ProfileIdentifier = "Profile";
    public UserProfile UserProfile;
    [Header("--- Database ---")]
    public MapDatabase[] maps;
    public TalentDatabase TalentDatabase;
    public StatSystem[] baseStatsHeroes;
    public InventorySystem InventorySystem = new InventorySystem();

    [Header("--- Value ---")]
    public int chosenMap = -1;
    private long consumedStaminaAmount;
    public int ChosenMap { get => chosenMap; set => chosenMap = value; }
    public long ConsumedStaminaAmount { get => consumedStaminaAmount; set => consumedStaminaAmount = value; }

    public void ChooseMap(int map)
    {
        chosenMap = map;
    }

    public MapDatabase GetMapDatabase()
    {
        return maps[chosenMap];
    }

    public int GetChosenMap()
    {
        return chosenMap;
    }

    public void UserProfileSetting()
    {
        SaveGame.DeleteAll();
        PlayerPrefs.DeleteAll();

        UserProfile = SaveGame.Exists(ProfileIdentifier) ? SaveGame.Load<UserProfile>(ProfileIdentifier) : new UserProfile(PlayerID());
    }

    private string PlayerID(){
        string id = "";
        for(int i=0; i< 10; i++){
            int rand = Random.Range(0, 10);
            id += rand.ToString();
        }
        return id;
    }

    public void SetChapterOpen()
    {
        if(chosenMap + 1 > UserProfile.chapterIndex)
            UserProfile.chapterIndex = chosenMap + 1;
    }

    
}
