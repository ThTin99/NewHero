using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class EnergyManager : Singleton<EnergyManager>
{
    [SerializeField] TMP_Text energyTxt;

    private readonly int maxEnergy = 60;
    private int CurrentEnergy => GameFlowManager.Instance.UserProfile.Energy;
    private readonly int restoreDuration = 5;
    private DateTime nextEnergyTime;
    private DateTime lastEnergyTime;
    private bool isRestoring = false;

    // Start is called before the first frame update
    void Start()
    {
        Load();
        StartCoroutine(RestoreEnergy());
    }

    //TODO: REMOVE THIS TEST FUNCTION
    public void AddEnergy()
    {
        GameFlowManager.Instance.UserProfile.Energy += 100;
    }

    public bool UseEnergy(int amount)
    {
        if (CurrentEnergy >= amount)
        {
            GameFlowManager.Instance.UserProfile.UpdateCurrency(ItemType.ENERGY, -amount);
            UpdateEnergy();

            if (!isRestoring)
            {
                if (CurrentEnergy + 1 <= maxEnergy)
                {
                    nextEnergyTime = AddDuration(DateTime.Now, restoreDuration);
                }

                StartCoroutine(RestoreEnergy());
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator RestoreEnergy()
    {
        isRestoring = true;

        while (CurrentEnergy < maxEnergy)
        {
            var currentDatime = DateTime.Now;
            var nextDateTime = nextEnergyTime;
            var isEnergyAdding = false;

            while (currentDatime > nextDateTime)
            {
                if (CurrentEnergy < maxEnergy)
                {
                    isEnergyAdding = true;
                    GameFlowManager.Instance.UserProfile.Energy++;
                    UpdateEnergy();
                    var timeToAdd = lastEnergyTime > nextDateTime ? lastEnergyTime : nextDateTime;
                    nextDateTime = AddDuration(timeToAdd, restoreDuration);
                }
                else
                {
                    break;
                }
            }

            if (isEnergyAdding)
            {
                lastEnergyTime = DateTime.Now;
                nextEnergyTime = nextDateTime;
            }

            UpdateEnergy();
            Save();
            yield return null;
        }

        isRestoring = false;
    }

    private DateTime AddDuration(DateTime dateTime, int duration)
    {
        return dateTime.AddMinutes(duration);
    }

    private void UpdateEnergy()
    {
        energyTxt.text = CurrentEnergy + " / " + maxEnergy;
    }

    private DateTime StringToDate(string dateTime)
    {
        if (string.IsNullOrEmpty(dateTime))
        {
            return DateTime.Now;
        }
        else
        {
            return DateTime.Parse(dateTime);
        }
    }

    private void Load()
    {
        nextEnergyTime = StringToDate(PlayerPrefs.GetString("nextEnergyTime"));
        lastEnergyTime = StringToDate(PlayerPrefs.GetString("lastEnergyTime"));

        UpdateEnergy();
    }

    private void Save()
    {
        PlayerPrefs.SetString("nextEnergyTime", nextEnergyTime.ToString());
        PlayerPrefs.SetString("lastEnergyTime", lastEnergyTime.ToString());
    }
}
