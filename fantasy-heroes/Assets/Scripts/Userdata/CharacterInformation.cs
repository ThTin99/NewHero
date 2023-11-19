[System.Serializable]
public class CharacterInformation
{
    public CharaterID CharaterID;
    public int Level;
    public CharacterStatus CharacterStatus;
    public int cost;

    public CharacterInformation(CharaterID charaterID, int level, CharacterStatus characterStatus, int Cost)
    {
        CharaterID = charaterID;
        Level = level;
        CharacterStatus = characterStatus;
        cost = Cost;
    }
}

public enum CharaterID { MaleArcher, MaleKnight, FemaleArcher, FemaleMage, FemaleKnight, MageMale}