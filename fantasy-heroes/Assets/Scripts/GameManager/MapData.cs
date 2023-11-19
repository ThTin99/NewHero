using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData")]
public class MapData : ScriptableObject
{
    public List<Map> maps = new List<Map>();
}

[System.Serializable]
public class Map
{
    public List<ObtaclesScript> obstacles = new List<ObtaclesScript>();
    public List<ObstaclePositions> spawnPositions = new List<ObstaclePositions>();
}

[System.Serializable]
public struct ObstaclePositions
{
    public Vector3[] positions;
}