using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderAI : MonoBehaviour
{
    public float moveSpeed;
    [SerializeField] float attackTime = 1;
    [SerializeField] int idleTime = 1;
    float freezeTime = 2;
    //float rotSpeed = 1f;
    bool isWandering = false;
    bool isWalking = false;
    private int[] _occurListSample = new int[] {1, 2, 3, 4};
    private List<int> _occurList = new List<int>();
    private Collider m_Collider;
    
    Enemy enemy;
    public bool isOnlyFollow = false;
    private void Awake() {
        m_Collider = GetComponent<Collider>();
        enemy = GetComponent<Enemy>();
    }

    private void Start() {
        ChangeRotation(180f);
        CreateRandomList();
    }

    void CreateRandomList(){
        for(int i = 0; i< 33;i++)
        {
            for(int j=0; j<_occurListSample.Length; j++){
                _occurList.Add(_occurListSample[j]);
            }
        }
        Shuffle(_occurList);
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

    private void Update() {

        if(freezeTime > 0){
            freezeTime -= Time.deltaTime;
            return;
        }
        
        if(isWandering == false && !enemy.isDie && enemy._player.characterState != CharacterState.DEAD){
            StartCoroutine(Wander());
        }
        
        if(isWalking && !enemy.isDie){
            transform.position += (transform.forward * moveSpeed * Time.deltaTime);
            enemy.MoveAnimation();
            if(enemy.weaponType == NPC_WeaponType.MELEE){
                //MELEE ATTACK WHEN MOVING
                if(enemy.CheckDistanceToAttack()){
                    Attacking();
                }
            }

        }
    }

    IEnumerator Wander(){

        int moveTime = _occurList[Random.Range(0,_occurList.Count)];
        isWandering = true;

        if(idleTime > 0){
            //IDLE
            enemy.IdleAnimation();

            if(enemy.CheckDistanceToAttack() && enemy.weaponType == NPC_WeaponType.MELEE){
                //CountToRangeAttack = 0 -> MELEE ATTACK
                Attacking();
            }
            yield return new WaitForSeconds(idleTime);
        }
        

        if(attackTime > 0){
            //ATTACK
            Attacking();
            yield return new WaitForSeconds(attackTime);
        }

        if(moveSpeed > 0){
            //ROTATING && MOVING
            StartCoroutine(Rotating());
            yield return new WaitForSeconds(0.1f);
            Moving();
            yield return new WaitForSeconds(moveTime);
        }
        
        isWalking = false;
        isWandering = false;
    }

    IEnumerator Rotating(float time = 0.1f){
        if(!isOnlyFollow){
            int rotateLorR = _occurList[Random.Range(0,_occurList.Count)];
            if(rotateLorR == 1){
                ChangeRotation(Random.Range(0,90));
                yield return new WaitForSeconds(time);
            }
            if(rotateLorR == 2){
                ChangeRotation(Random.Range(-90,0));
                yield return new WaitForSeconds(time);
            }
            if(rotateLorR >= 3){
                transform.LookAt(enemy.player.transform);
                ChangeRotation(0);
            }
        }else{
            transform.LookAt(enemy.player.transform);
            ChangeRotation(0);
        }
    }

    void Moving(){
        isWalking = true;
        enemy.MoveAnimation();
    }

    void ChangeRotation(float value){
        Vector3 temp = transform.rotation.eulerAngles;
        temp.x = 0;
        temp.z = 0;
        temp.y += value;
        Quaternion rotation = Quaternion.Euler(temp);
        transform.rotation = rotation;
    }

    void Attacking(){
        enemy.AttackAnimation();
        isWalking = false;
        Invoke("EndAttack", attackTime);
    }

    void EndAttack(){
        isWalking = true;
    }

    void OnCollisionEnter(Collision collision) {
        ReflectMove(collision);
        ButtDamaged(collision);
    }

    void ReflectMove(Collision collision){
        if (collision.gameObject.CompareTag("Obtacles") || collision.gameObject.CompareTag("Obstacles_Not_Through")){
            Vector3 newDirection = Vector3.Reflect(transform.forward, collision.contacts[0].normal);
            transform.rotation = Quaternion.LookRotation(newDirection);
            ChangeRotation(0);
        } 
    }

    void ButtDamaged(Collision collision){
        if(enemy.weaponType == NPC_WeaponType.BUTT || enemy.weaponType == NPC_WeaponType.BUTT_RANGE){
            if (collision.gameObject.CompareTag("Player")){
                IDamageable Damageable = collision.gameObject.GetComponent<IDamageable>();
                enemy.enemyAttackController.ButtAttack(Damageable);
            }
        }
    }
}

