using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class PortalSpawnScript : MonoBehaviour
{
    [SerializeField] Enemy enemyPrefab;
    [SerializeField] Bullet bulletPrefab;
    public bool isSpawnEnemy;
    public Quaternion rotation;
    private void Start() {
        Invoke("Spawn", 1f);
    }
    private void Spawn() {
        Spawning(isSpawnEnemy);
    }
    void Spawning(bool _isSpawnEnemy){
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        if(_isSpawnEnemy){
            spawnPos.y = 0f;
            Enemy e = LeanPool.Spawn(enemyPrefab, spawnPos, rotation, GameManager.Instance.monsterHolder);
            GameManager.Instance.enemyList.Add(e);
        }else{
            Bullet b = LeanPool.Spawn(bulletPrefab, spawnPos, rotation, GameManager.Instance.bulletHolder);
        }
    }
}
