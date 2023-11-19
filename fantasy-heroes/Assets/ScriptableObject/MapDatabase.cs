using UnityEngine;

[CreateAssetMenu(menuName = "New MapDatabase")]
public class MapDatabase : ScriptableObject
{
    public MapScript mapAsset;
    public MonsterWaveScript[] monsterWaves;
    public MapData obstacleData;
    public Vector3 playerSpawmPosition;
}
