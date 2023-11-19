using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Talent Database", menuName = "Database/Talent Database")]
public class TalentDatabase : ScriptableObject
{
    public List<TalentUnit> TalentUnits;
}
