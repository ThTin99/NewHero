using Lean.Pool;
using System.Collections;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    [SerializeField] ParticleSystem powerFX,powerFX2,subPowerFX;

    [SerializeField] Bullet powerBullet;
    [SerializeField] private bool isTutorialCharacter;

    public Bullet bullet;
    private readonly float timeToPower = 2f;
    private const float barrageShootZ = 1f;
    private Character character;
    private Transform bulletHolder;
    private float timePowerCount = 0;
    internal bool isPowerShot = false;
    private bool isMove = false;
    private CharaterID m_CharaterID;
    internal bool isPowerShotting = false;

    private void Awake()
    {
        character = GetComponent<Character>();
        m_CharaterID = GameFlowManager.Instance.UserProfile.selectedHero;
    }

    private void Update()
    {
        if (isMove && !isPowerShot && isTutorialCharacter == false)
        {
            timePowerCount += Time.deltaTime;
            if(character.characterData.energyBar != null)  character.characterData.energyBar.EnergySetting(timeToPower, timePowerCount);
            if (timePowerCount >= 2)
            {
                FullPower();
            }
        }
        else
        {
            if (timePowerCount < timeToPower)
            {
                timePowerCount = 0;
                if(character.characterData.energyBar != null) character.characterData.energyBar.EnergySetting(timeToPower, timePowerCount);
            }
        }
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);
        bulletHolder = GameManager.Instance.bulletHolder;
    }

    public void Shoot()
    {
        if (!character.targetEnemy) return;
        character.RotateTowardEnemy();
        character.weaponPivot.LookAt(character.targetEnemy.mindPoint.position);

        if (isPowerShot)
        {
            StartCoroutine(PowerShot());
            isPowerShot = false;
            timePowerCount = 0;
            character.characterData.energyBar.EnergySetting(timeToPower, timePowerCount);
        }
        else
        {
            NormalShot();
        }
    }

    public void CheckingMove(bool _isMove)
    {
        isMove = _isMove;
    }

    void FullPower()
    {
        isPowerShot = true;
        timePowerCount = 2;
        powerFX.gameObject.SetActive(true);
        powerFX.Play();
        if(powerFX2 != null)
        {
            powerFX2.gameObject.SetActive(true);
            powerFX2.Play();
        }
    }

    IEnumerator PowerShot()
    {
        isPowerShotting = true;
        powerFX.Stop();
        powerFX.gameObject.SetActive(false);
        if (powerFX2 != null)
        {
            powerFX2.Stop();
            powerFX2.gameObject.SetActive(false);
        }

        Vector3 temp = character.weaponPivot.rotation.eulerAngles;
        temp.x = 0;
        temp.z = 0;
        Quaternion rotation = Quaternion.Euler(temp);

        if (m_CharaterID == CharaterID.MaleArcher)
        {
            float[] Array = new float[] {1f , 0f, -1f};
            for (int i = 0; i < 12; i++)
            {
                int rand = Random.Range(0, Array.Length);
                LeanPool.Spawn(powerBullet, character.weaponPivot.position + new Vector3(Array[rand], 0f), rotation, bulletHolder);
                yield return new WaitForSeconds(0.05f);
            }
        }
        else if(m_CharaterID == CharaterID.FemaleArcher)
        {
            subPowerFX.Play();
            subPowerFX.gameObject.SetActive(true);
            temp.y = 0;
            temp.x = 15;
            for (int i = 0; i < 10; i++)
            {
                temp.y += 36f;
                rotation = Quaternion.Euler(temp);
                LeanPool.Spawn(powerBullet, character.weaponPivot.position, rotation, bulletHolder);
                yield return new WaitForSeconds(0.005f);
            }
        }
        else if (m_CharaterID == CharaterID.MageMale) 
        {
            float[] Array = new float[] { -4f, 4f };
            Vector3 pos1 = character.weaponPivot.position + new Vector3(Array[0], 0f, 0f);
            Vector3 pos2 = character.weaponPivot.position + new Vector3(Array[1], 0f, 0f);
            for (int i = 0; i < 5; i++)
            {
                if (character.targetEnemy != null)
                {
                    Bullet bullet = LeanPool.Spawn(powerBullet, character.weaponPivot.position, rotation, bulletHolder);
                    bullet.DirectToTarget(character.targetEnemy.mindPoint.position);
                    Bullet bullet1 = LeanPool.Spawn(powerBullet, pos1, rotation, bulletHolder);
                    bullet1.DirectToTarget(character.targetEnemy.mindPoint.position);
                    Bullet bullet2 = LeanPool.Spawn(powerBullet, pos2, rotation, bulletHolder);
                    bullet2.DirectToTarget(character.targetEnemy.mindPoint.position);
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
        else if (m_CharaterID == CharaterID.FemaleKnight) 
        {
            temp.y += 15f;
            for(int i=0; i<3; i++){
                rotation = Quaternion.Euler(temp);
                LeanPool.Spawn(powerBullet, character.weaponPivot.position, rotation, bulletHolder);
                temp.y -= 15f;
            }
        }
        else
        {
            LeanPool.Spawn(powerBullet, character.weaponPivot.position, rotation, bulletHolder);
            character.ShowEnchant(false);
        }
        isPowerShotting = false;
    }

    void NormalShot()
    {
        GameManager.Instance.soundManager.PlayOnShotSound("ReleasingBow");
        GameManager.Instance.soundManager.PlayOnShotSound("ArrowFlying");
        FrontShot();
        if (character.Barrage)
        {
            BarrageShot();
        }

        if (character.AngleShot)
        {
            AngleShot();
        }

        if (character.SideShot)
        {
            SideShot();
        }
    }

    private void BarrageShot()
    {
        Vector3 temp = character.weaponPivot.rotation.eulerAngles;
        temp.x = 0;
        temp.z = 0;
        Quaternion rotation = Quaternion.Euler(temp);
        switch (character.ProjectileLevel)
        {
            case 2:
                LeanPool.Spawn(bullet, character.weaponPivot.position + new Vector3(0.4f, 0, barrageShootZ), rotation, bulletHolder);
                LeanPool.Spawn(bullet, character.weaponPivot.position + new Vector3(-0.4f, 0, barrageShootZ), rotation, bulletHolder);
                break;
            case 3:
                LeanPool.Spawn(bullet, character.weaponPivot.position + new Vector3(-0.8f, 0, barrageShootZ), rotation, bulletHolder);
                LeanPool.Spawn(bullet, character.weaponPivot.position + new Vector3(0, 0, barrageShootZ), rotation, bulletHolder);
                LeanPool.Spawn(bullet, character.weaponPivot.position + new Vector3(0.8f, 0, barrageShootZ), rotation, bulletHolder);
                break;
            default:
                LeanPool.Spawn(bullet, character.weaponPivot.position + new Vector3(0, 0, barrageShootZ), rotation, bulletHolder);
                break;
        }
    }

    private void SideShot()
    {
        Vector3 temp = character.weaponPivot.rotation.eulerAngles;
        temp.x = 0;
        temp.z = 0;
        temp.y -= 90f;
        Quaternion rotation1 = Quaternion.Euler(temp);
        LeanPool.Spawn(bullet, character.weaponPivot.position, rotation1, bulletHolder);
        temp.y += 180f;
        Quaternion rotation2 = Quaternion.Euler(temp);
        LeanPool.Spawn(bullet, character.weaponPivot.position, rotation2, bulletHolder);
    }

    private void AngleShot()
    {
        Vector3 temp = character.weaponPivot.rotation.eulerAngles;
        temp.x = 0;
        temp.z = 0;
        temp.y -= 15f;
        Quaternion rotation1 = Quaternion.Euler(temp);
        LeanPool.Spawn(bullet, character.weaponPivot.position, rotation1, bulletHolder);
        temp.y += 30f;
        Quaternion rotation2 = Quaternion.Euler(temp);
        LeanPool.Spawn(bullet, character.weaponPivot.position, rotation2, bulletHolder);
    }

    private void FrontShot()
    {
        Vector3 temp = character.weaponPivot.rotation.eulerAngles;
        temp.x = 0;
        temp.z = 0;
        Quaternion rotation = Quaternion.Euler(temp);
        switch (character.ProjectileLevel)
        {
            case 2:
                LeanPool.Spawn(bullet, character.weaponPivot.position + new Vector3(0.4f, 0f), rotation, bulletHolder);
                LeanPool.Spawn(bullet, character.weaponPivot.position + new Vector3(-0.4f, 0f), rotation, bulletHolder);
                break;
            case 3:
                LeanPool.Spawn(bullet, character.weaponPivot.position + new Vector3(-0.8f, 0f), rotation, bulletHolder);
                LeanPool.Spawn(bullet, character.weaponPivot.position, rotation, bulletHolder);
                LeanPool.Spawn(bullet, character.weaponPivot.position + new Vector3(0.8f, 0f), rotation, bulletHolder);
                break;
            default:
                LeanPool.Spawn(bullet, character.weaponPivot.position, rotation, bulletHolder);
                break;
        }
    }


}
