using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data Quest", menuName = "Database/Data Quest")]
public class DataQuest : ScriptableObject
{
    public List<UnitQuest> dailyQuest;
}
