using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyAttackController : MonoBehaviour
{
    private Enemy enemy;
    public GameObject spawnFX;
    [Header("Melee Attack Settings")]
    public bool isJump;
    Vector3 targetJump;
    public int meleeDamage;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Range Attack Settings")]
    public Bullet projectilePrefab;
    public bool isRangeAngle, isBoundProjectile, isPredicted, isBackTurn;
    public int bulletCount;
    [Header("Spit Attack Settings")]
    public ParticleSystem[] spitBullet;
    [Header("Summoner Attack Settings")]
    public PortalSpawnScript portal;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void Start() {
        if(spawnFX != null){
            LeanPool.Spawn(spawnFX, transform.position, Quaternion.identity, GameManager.Instance.bulletHolder);
        }
    }

    private void Update()
    {
        if (enemy.isDie) return;
    }

    public void ButtAttack(IDamageable damageable){
        damageable.Damage(meleeDamage);
    }

    public void ButtRangeAttack(){
        Vector3 temp = enemy.weaponPivot.rotation.eulerAngles;
        temp.x = 0;
        temp.z = 0;
        Quaternion rotation = Quaternion.Euler(temp);
        int angle = 0;
        if(bulletCount == 4){
            angle = 90;
        }
        if(bulletCount == 8){
            angle = 45;
        }
        for(int i=0; i<bulletCount;i++){
            CreateBullet(rotation, enemy.weaponPivot.position);
            temp.y += angle;
            rotation = Quaternion.Euler(temp);
        }
    }

    public void MeleeAttack()
    {
        Vector3 temp = enemy.weaponPivot.rotation.eulerAngles;
        temp.x = 0;
        temp.z = 0;
        Quaternion rotation = Quaternion.Euler(temp);
        CreateBullet(rotation, enemy.weaponPivot.position);
    }

    public void TwistAttack(){
        int[] _occurList = new int[] {6, 8, 10, 12};
        bulletCount = _occurList[Random.Range(0,_occurList.Length)];
        float angle = 360/bulletCount;
        Vector3 temp = enemy.weaponPivot.rotation.eulerAngles;
        temp.x = 0;
        temp.y = 0;
        temp.z = 0;
        for(int i=0; i<bulletCount; i++){
            Quaternion rotation = Quaternion.Euler(temp);
            CreateBullet(rotation, enemy.weaponPivot.position);
            temp.y += angle;
        }
    }

    public void Spit(){
        for(int i=0; i< spitBullet.Length;i++){
            spitBullet[i].Play();
            spitBullet[i].gameObject.SetActive(true);
        }
    }

    public void StopSpit(){
        for(int i=0; i< spitBullet.Length;i++){
            spitBullet[i].Stop();
            spitBullet[i].gameObject.SetActive(false);
        }
    }

    public void MultiRangeAttack(){
        Quaternion playerRotation;
        playerRotation = Quaternion.Euler(new Vector3(0,0,0));
        if(enemy._player.transform.position.z < transform.position.z){
            playerRotation = Quaternion.Euler(new Vector3(0,180,0));
        }
        transform.rotation = playerRotation;

        List<Vector3> posArray = new List<Vector3>();
        for(int i=0; i< 9; i++){
            Vector3 pos;
            if(i <= 4){
                pos = new Vector3 (enemy.weaponPivot.position.x - (4 * 2.2f) + (i * 2.2f), enemy.weaponPivot.position.y, 
                                        enemy.weaponPivot.position.z + (4 * 2.2f) - (i * 2.2f));
                posArray.Add(pos); 
            }

            if(i > 4){
                pos = new Vector3 (enemy.weaponPivot.position.x - (4 * 2.2f) + (i * 2.2f), enemy.weaponPivot.position.y, 
                                        enemy.weaponPivot.position.z - (4 * 2.2f) + (i * 2.2f));
                posArray.Add(pos); 
            }           
                    
        }

        Vector3 temp = enemy.weaponPivot.rotation.eulerAngles;
        temp.x = 0;
        temp.z = 0;
        Quaternion rotation = Quaternion.Euler(temp);

        string[] _stringList = new string[] {"isCenterAttack", "isRightAttack" , "isLeftAttack"};
        int index = 0;
        for(int i=0; i< _stringList.Length; i++){
            if(enemy.npcAnimator.GetBool(_stringList[i])){
                index = i;
            }
        }

        switch (_stringList[index])
        {
            case "isCenterAttack":
                for(int i=2; i< 7; i++ ){
                    CreateBullet(rotation, posArray[i]);
                }
                break;
            case "isLeftAttack":
                for(int i=4; i< 9; i++ ){
                    CreateBullet(rotation, posArray[i]);
                }
                break;
            case "isRightAttack":
                for(int i=0; i< 5; i++ ){
                    CreateBullet(rotation, posArray[i]);
                }
                break;      
        }
    }

    public void RangedAttack()
    {
        Vector3 temp = enemy.weaponPivot.rotation.eulerAngles;
        temp.x = 0;
        temp.z = 0;
        Quaternion rotation = Quaternion.Euler(temp);
        //Shot 1
            CreateBullet(rotation, enemy.weaponPivot.position);
            
            if(bulletCount == 1) return;

            if(isRangeAngle){       
                temp.y -= 5f;
                rotation = Quaternion.Euler(temp);
            }
            //Shot 2
            CreateBullet(rotation, enemy.weaponPivot.position);
            
            if(isRangeAngle)  {          
                temp.y += 10f;
                rotation = Quaternion.Euler(temp);
            }
            //Shot 3
            CreateBullet(rotation, enemy.weaponPivot.position);

            if(bulletCount == 3) return;

            if(isRangeAngle){       
                temp.y -= 15f;
                rotation = Quaternion.Euler(temp);
            }
            //Shot 4
            CreateBullet(rotation, enemy.weaponPivot.position);

            if(isRangeAngle){       
                temp.y += 20f;
                rotation = Quaternion.Euler(temp);
            }
            //Shot 5
            CreateBullet(rotation, enemy.weaponPivot.position);

            if(bulletCount == 5) return;

            if(isRangeAngle){       
                temp.y -= 25f;
                rotation = Quaternion.Euler(temp);
            }
            //Shot 6
            CreateBullet(rotation, enemy.weaponPivot.position);

            if(isRangeAngle){       
                temp.y += 30f;
                rotation = Quaternion.Euler(temp);
            }
            //Shot 7
            CreateBullet(rotation, enemy.weaponPivot.position);

            if(bulletCount == 7) return;
    }

    public void RandomRangedAttack(){
        LookAtSetting();
        enemy.weaponPivot.LookAt(enemy._player.mindPoint.position);
        Vector3 temp = enemy.weaponPivot.rotation.eulerAngles;
        Quaternion rotation = Quaternion.Euler(temp);
        CreateBullet(rotation, enemy.weaponPivot.position);
    }

    void LookAtSetting(){
        transform.LookAt(enemy._player.mindPoint.position);
        Vector3 temp = transform.rotation.eulerAngles;
        temp.x = 0;
        temp.z = 0;
        Quaternion rotation = Quaternion.Euler(temp);
        transform.rotation = rotation;
    }

    public void SummonAttack(){

        Quaternion playerRotation;
        playerRotation = Quaternion.Euler(new Vector3(0,0,0));
        if(enemy._player.transform.position.z < transform.position.z){
            playerRotation = Quaternion.Euler(new Vector3(0,180,0));
        }
        transform.rotation = playerRotation;

        int summonMonster = 30;
        int rand = Random.Range(0,100);
        Vector3 SpawnPos = new Vector3(enemy.weaponPivot.position.x - 8f, 0f, enemy.weaponPivot.position.z);
        Vector3 temp = enemy.weaponPivot.rotation.eulerAngles;
        temp.x = 0;
        temp.z = 0;
        temp.y = 0;
        for(int i=0; i<5;i++){
            if(rand <= summonMonster){
                Quaternion rotation = Quaternion.Euler(temp);
                PortalSpawnScript p = LeanPool.Spawn(portal, SpawnPos, rotation, GameManager.Instance.bulletHolder);
                p.isSpawnEnemy = true;
                p.rotation = playerRotation;
            }else{
                temp.y = 90;
                temp.z = 90;
                SpawnPos.y = 2f;
                Quaternion rotation = Quaternion.Euler(temp);
                PortalSpawnScript p = LeanPool.Spawn(portal, SpawnPos, rotation, GameManager.Instance.bulletHolder);
                p.isSpawnEnemy = false;
                p.rotation = playerRotation;
            }
            SpawnPos.x += 4f;
        }
        
    }

    void CreateBullet(Quaternion rot, Vector3 pos){
        Bullet bullet = LeanPool.Spawn(projectilePrefab, pos, rot, GameManager.Instance.bulletHolder);
        bullet.isBound = isBoundProjectile;
        bullet.isPredicted = isPredicted;
        bullet.isBackTurn = isBackTurn;
        if(isPredicted){
            bullet.transform.DOJump(targetJump,10f,1,1.5f,false).SetEase(Ease.OutSine);
        }
    }

    public void GetTarget(){
        targetJump = enemy.player.transform.position;
    }

    public void JumpAction(){
        if(isJump){
            if(lineRenderer != null) lineRenderer.enabled = false;
            float directionX = 1;
            float directionZ = 1;
            if(transform.position.x > targetJump.x){
                directionX = -1;
            }
            if(transform.position.z > targetJump.z){
                directionZ = -1;
            }
            Vector3 pos = targetJump - new Vector3(2 * directionX,0,2 * directionZ);
            transform.DOJump(pos,1.5f,1,0.5f,false).SetEase(Ease.OutQuad);
        }
    }

    public void GetTargetJumpPosition(){
        GetTarget();
        if(lineRenderer != null){
            lineRenderer.enabled = true; 
            lineRenderer.SetPosition(0, transform.position + new Vector3(0f,0.5f,0f));
            lineRenderer.SetPosition(1, targetJump + new Vector3(0f,0.5f,0f));
        }
    }

    void Shuffle(List<int> list){
        var count = list.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }
}


