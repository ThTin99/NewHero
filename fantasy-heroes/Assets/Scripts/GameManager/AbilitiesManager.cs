using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AbilitiesManager : Singleton<AbilitiesManager>
{
    [Header("List of available abilities")]
    [SerializeField]
    internal List<Ability> abilities;

    [Header("Database")]
    [SerializeField]
    private AbilitiesDatabase AbilitiesDatabase;

    [Header("Split projectile addressable")]
    [SerializeField]
    private GameObject splitProjectileAsset;


    [Header("Ice ball addressable")]
    [SerializeField]
    private GameObject iceBallAsset;

    [Header("Workshop cards")]
    [SerializeField]
    private AbilityCard[] workshopAbilityCards;
    [SerializeField]
    private TMPro.TMP_Text currentHpTxt;

    private List<int> abilityIndexs = new List<int>();

    private void Start()
    {
        CreateListOfAvailableAbilityIndex();

        abilities = new List<Ability>();
    }

    private void CreateListOfAvailableAbilityIndex()
    {
        for (int i = 0; i < AbilitiesDatabase.abilities.Count; i++)
        {
            abilityIndexs.Add(i);
        }
    }

    public void AddAbility(AbilityCard abilityCard, bool isWorkshopCard = false)
    {
        if (isWorkshopCard)
        {
            abilities.Add(AbilitiesDatabase.workshopAbility.Find(x => x.abilityType == abilityCard.CardData.abilityType));
        }
        else
        {
            var abilityIndexInDatabase = AbilitiesDatabase.abilities.FindIndex(0, x => x.abilityType == abilityCard.CardData.abilityType);
            abilities.Add(AbilitiesDatabase.abilities[abilityIndexInDatabase]);

            abilityIndexs.Remove(abilityIndexInDatabase);

            //reset count to actual number
            abilityIndexs.TrimExcess();
        }
    }

    internal Ability GetRandomAbility()
    {
        //#if UNITY_EDITOR
        //        return AbilitiesDatabase.abilities[AbilitiesDatabase.abilities.Count - 1]; //for testing newest ability
        //#endif
        //take random index
        var index = Random.Range(0, abilityIndexs.Count);

        //take index before remove
        return AbilitiesDatabase.abilities[abilityIndexs[index]];
    }

    public void SplitShoot(float colliderSize, Vector3 position, Quaternion rotation)
    {
        SpawnSplitShot(colliderSize, position, rotation);
    }

    private void SpawnSplitShot(float colliderSize, Vector3 position, Quaternion rotation)
    {
        Vector3 temp = rotation.eulerAngles;
        temp.x = 0;
        temp.z = 0;
        temp.y += 90;
        Quaternion _rotation = Quaternion.Euler(temp);
        Bullet _Splitbullet1 = Lean.Pool.LeanPool.Spawn(splitProjectileAsset,
           new Vector3(position.x - colliderSize, position.y, position.z),
                    _rotation).GetComponent<Bullet>();
        temp.y -= 180;
        _rotation = Quaternion.Euler(temp);
        Bullet _Splitbullet2 = Lean.Pool.LeanPool.Spawn(splitProjectileAsset,
            new Vector3(position.x + colliderSize, position.y, position.z),_rotation).GetComponent<Bullet>();
        _Splitbullet1.isSplited = true;
        _Splitbullet2.isSplited = true;
    }

    public void SpawnIceBall()
    {
       Lean.Pool.LeanPool.Spawn(iceBallAsset);
    }

    public void UpdateWorkshopPanel()
    {
        UpdateCurrentHpTxt();
        UpdateCards();
    }

    private void UpdateCards()
    {
        workshopAbilityCards[0].SetInfo(GetLeftWorkshopCardAbility());
        workshopAbilityCards[1].SetInfo(GetRightWorkshopCardAbility());
    }

    private void UpdateCurrentHpTxt()
    {
        var playerStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterData>().Stats;
        currentHpTxt.text = string.Format("HP ({0}/{1})", playerStatus.CurrentHealth, playerStatus.CurrentMaxHealth);
    }

    private Ability GetLeftWorkshopCardAbility()
    {
        return AbilitiesDatabase.workshopAbility[0];
    }

    private Ability GetRightWorkshopCardAbility()
    {
        return AbilitiesDatabase.workshopAbility[1];
    }
}
