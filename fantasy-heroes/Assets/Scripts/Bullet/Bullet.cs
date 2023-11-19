/*This script created by using docs.unity3d.com/ScriptReference/MonoBehaviour.OnParticleCollision.html*/
using DG.Tweening;
using Lean.Pool;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject hit, flash, main;
    float timeToShowMain;
    private Rigidbody rb;
    Collider m_Collider;
    public float speed = 15f;
    public float hitOffset = 0f;
    public bool keepOnObtacles, UseFirePointRotation;
    public Vector3 rotationOffset = new Vector3(0, 0, 0);
    public float destroyTime;
    public int baseDamage;
    [SerializeField] internal int currentDamage;
    private Character player;
    private Vector3 oldVelocity;
    Transform bulletHolder;
    bool isNotUseVelocity;
    int boundCount = 0;
    public bool isPredicted, isBound, isSplited, dealthBlow, isBackTurn, isAutoChangeSpeed, isFragmentation;
    public Bullet fragmentationBullet;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
        currentDamage = baseDamage;
        bulletHolder = GameManager.Instance.bulletHolder;
        ApplyDamage();
        initAlready = true;
        if (main != null) main.SetActive(false);
        //Bullet Transform
        rb = GetComponent<Rigidbody>();
        m_Collider = GetComponent<Collider>();

        if(isAutoChangeSpeed){
            AutoChangeSpeed();
        }

        if (flash != null)
        {
            var flashInstance = LeanPool.Spawn(flash, transform.position, Quaternion.identity, bulletHolder);
            flashInstance.transform.forward = gameObject.transform.forward;
            var flashPs = flashInstance.GetComponent<ParticleSystem>();
            if (flashPs != null)
            {
                LeanPool.Despawn(flashInstance, flashPs.main.duration);
            }
            else
            {
                var flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                LeanPool.Despawn(flashInstance, flashPsParts.main.duration);
            }
        }
        if(destroyTime > -1){
            Destroy(gameObject, destroyTime);
        }
    }

    private bool initAlready;

    private void AutoChangeSpeed(){
        int rand = Random.Range(0,3);
        if(rand == 0){
            speed += 5;
        }else if(rand == 1){
            speed -= 3;
        }
    }

    private void OnEnable()
    {
        if (initAlready)
        {
            ApplyDamage();
        }
    }

    void FixedUpdate()
    {
        if (!isPredicted && !isNotUseVelocity)
        {
            rb.velocity = transform.forward * speed;
            oldVelocity = rb.velocity;
        }

        if (main != null)
        {
            timeToShowMain += Time.deltaTime;
            if (timeToShowMain >= 0.01f)
            {
                main.SetActive(true);
            }
        }
    }

    public void DirectToTarget(Vector3 _target){
        transform.LookAt(_target);
    }

    private void ApplyDamage()
    {
        if (gameObject.CompareTag("Bullet") && player != null)
        {
            if (player.DealthBlow)
            {
                dealthBlow = true;
            }
            int WeaponDamage = 0;
            if(GameFlowManager.Instance.UserProfile.Weapon != null){
                 WeaponDamage = GameFlowManager.Instance.UserProfile.Weapon.GetRandomDmg();
            }
            var newDmg = baseDamage + WeaponDamage + player.characterData.Stats.CurrentMaxStrength;
            //if nothing change, return
            if (newDmg == currentDamage)
            {
                return;
            }
            currentDamage = newDmg;

            if (player.FullCharge && player.characterData.Stats.CurrentHealth > player.characterData.Stats.CurrentMaxHealth * 80 / 100)
            {
                currentDamage += player.characterData.Stats.CurrentMaxStrength * 60 / 100;
            }

            if (player.UnstableTech)
            {
                var randomStrengthPercent = Random.Range(10, 50);
                currentDamage += player.characterData.Stats.CurrentMaxStrength * randomStrengthPercent / 100;
            }

            if (isSplited)
            {
                currentDamage = currentDamage * 40 / 100;//splited bullet has 40% damage of the original
            }
        }
    }
    int idObtacles;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetInstanceID() == idObtacles)
        {
            return;
        }

        idObtacles = collision.gameObject.GetInstanceID();

        //Lock all axes movement and rotation
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point + contact.normal * hitOffset;

        if (hit != null)
        HitFX(pos, rot, contact); 
        
        InflictDamageTo(collision.gameObject);
        rb.velocity = Vector3.zero;

        if (collision.gameObject.CompareTag("Obtacles") || 
        collision.gameObject.CompareTag("Obstacles_Not_Through")) 
        HitObtacles(contact, collision);
        
        Freeze();
        DestroyBullet(collision);
    }

    void HitFX(Vector3 pos, Quaternion rot, ContactPoint contact){
        var hitInstance = LeanPool.Spawn(hit, pos, rot, bulletHolder);
            if (UseFirePointRotation) { hitInstance.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0); }
            else if (rotationOffset != Vector3.zero) { hitInstance.transform.rotation = Quaternion.Euler(rotationOffset); }
            else { hitInstance.transform.LookAt(contact.point + contact.normal); }

            var hitPs = hitInstance.GetComponent<ParticleSystem>();
            if (hitPs != null)
            {
                LeanPool.Despawn(hitInstance, hitPs.main.duration);
            }
            else
            {
                var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                LeanPool.Despawn(hitInstance, hitPsParts.main.duration);
            }
    }

    void HitObtacles(ContactPoint contact, Collision collision){
            if (isBound && boundCount <= 3)
            {
                Vector3 reflectedVelocity = Vector3.Reflect(oldVelocity, contact.normal);
                rb.velocity = reflectedVelocity;
                Quaternion rotation = Quaternion.FromToRotation(oldVelocity, reflectedVelocity);
                transform.rotation = rotation * transform.rotation;
                boundCount++;
                return;
            }

            if (isBackTurn)
            {
                transform.DOJump(player.transform.position, 10f, 1, 3f, false);
                isBackTurn = false;
                isNotUseVelocity = true;
                return;
            }

            if(isFragmentation){
                
                // Vector3 temp = new Vector3();
                
                // if (collision.gameObject.CompareTag("Obtacles"))
                // {
                //     temp = transform.rotation.eulerAngles;
                //     temp.x = 0;
                //     temp.z = 0;
                // }

                // if(collision.gameObject.CompareTag("Obstacles_Not_Through"))
                // {
                //     Vector3 reflectedVelocity = Vector3.Reflect(oldVelocity, contact.normal);
                //     rb.velocity = reflectedVelocity;
                //     Quaternion reflectRotation = Quaternion.FromToRotation(oldVelocity, reflectedVelocity);
                //     temp = reflectRotation.eulerAngles;
                //     temp.x = 0;
                //     temp.z = 0;
                // }

                // temp.y -= 20;
                // for(int i=0; i< 2; i++){
                //     Quaternion rotation = Quaternion.Euler(temp);
                //     Bullet b = LeanPool.Spawn(fragmentationBullet, transform.position, rotation, bulletHolder);
                //     temp.y += 40;
                // } 
            }
    }

    void Freeze(){
        m_Collider.enabled = false;
        rb.freezeRotation = true;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.angularVelocity = Vector3.zero;
    }

    void DestroyBullet(Collision collision){
        if (!keepOnObtacles) Destroy(gameObject);
        else
        {
            if (!collision.gameObject.CompareTag("Obtacles")) Destroy(gameObject);
            else Destroy(gameObject, 1);
        }
        if (main != null) main.SetActive(false);
    }

    private void OnTriggerEnter(Collider other) {
        InflictDamageTo(other.gameObject);
    }

    void InflictDamageTo(GameObject other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            var finalDamage = currentDamage;

            //do this with hero bullet
            if (CompareTag("Bullet"))
            {
                if (player.TsarBomba)
                {
                    finalDamage += currentDamage * 50 / 100;//deal more 50% current damage
                }

                if (player.SplitShot && !isSplited)//only split once, dont split power shot!
                {
                    if (other.TryGetComponent(out BoxCollider box))
                    {
                        AbilitiesManager.Instance.SplitShoot(box.size.x + 0.1f, other.transform.position, transform.rotation);
                    }
                    else if (other.TryGetComponent(out CapsuleCollider cap))
                    {
                        AbilitiesManager.Instance.SplitShoot(cap.radius + 0.1f, other.transform.position, transform.rotation);
                    }
                }

                if (other.TryGetComponent(out ICanReceiveElementalEffect elementalEffect))
                {
                    if (player.FireShot)
                    {
                        elementalEffect.ApplyEffect(new ElementalEffect(8f, StatSystem.DamageType.Fire, player.characterData.Stats.CurrentMaxStrength * 25 / 100));
                    }
                    if (player.ElectricShot)
                    {
                        elementalEffect.ApplyEffect(new ElementalEffect(4f, StatSystem.DamageType.Electric, player.characterData.Stats.CurrentMaxStrength * 50 / 100));
                    }
                    if (player.CorrodeShot)
                    {
                        elementalEffect.ApplyEffect(new ElementalEffect(2f, StatSystem.DamageType.Fire, player.characterData.Stats.CurrentMaxStrength * 70 / 100));
                    }
                }

                if (other.TryGetComponent(out ISlowable slowable))
                {
                    if (player.ColdShot)
                    {
                        slowable.ApplySlowEffect(50, 2);
                    }
                }
            }

            if (other.TryGetComponent(out CharacterData characterData))
            {
                if (!characterData.isDie)
                {
                    damageable.Damage(finalDamage, dealthBlow && Random.Range(0, 100.0f) <= 3.0f, player.TargetWeakPoint);
                }
                if (characterData.characterType != CharacterType.HERO) KnockBack(other);
            }
        }
    }

    void KnockBack(GameObject body)
    {
        float powerKnockBack = 1f;
        Enemy enemy = body.GetComponent<Enemy>();
        if (enemy.m_CharacterData.characterType == CharacterType.BOSS) return;
        Vector3 Direction = (enemy.transform.position - enemy.player.transform.position).normalized * powerKnockBack;
        enemy.GetComponent<Rigidbody>().AddForce(Direction, ForceMode.Impulse);
    }
}
