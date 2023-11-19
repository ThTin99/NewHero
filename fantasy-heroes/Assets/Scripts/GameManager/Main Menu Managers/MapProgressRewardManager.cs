using BayatGames.SaveGameFree;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapProgressRewardManager : MonoBehaviour
{
    [SerializeField] private MapProgressRewardDatabase database;

    [Header("Slider")]
    [SerializeField] private Slider slider;
    [SerializeField] private Image[] countBackgrounds;

    [Header("Reward tabs")]
    [SerializeField] private MapProgressRewardItem[] mapProgressRewardItems;

    [Header("Resources")]
    [SerializeField] private Sprite activeCountImg;
    [SerializeField] private Sprite inactiveCountImg;

    [Header("Main Menu References")]
    [SerializeField] private GameObject alarmIcon;
    [SerializeField] private Slider Slider;
    [SerializeField] private TMPro.TMP_Text levelProgressTxt;

    private List<MapProgressRewardItem.RewardState> rewardStates;
    private const string Key = "MapProgressRewards";

    private void Awake()
    {
        rewardStates = SaveGame.Load<List<MapProgressRewardItem.RewardState>>(Key);
        if (rewardStates == null)
        {
            rewardStates = new List<MapProgressRewardItem.RewardState>();
            for (int i = 0; i < GameFlowManager.Instance.UserProfile.processMapDatas.Count; i++)
            {
                rewardStates.Add(MapProgressRewardItem.RewardState.LOCKED);
            }
        }
    }

    private void Start()
    {
        float sliderValue = 0;
        for (int i = 0; i < mapProgressRewardItems.Length; i++)
        {
            if (GameFlowManager.Instance.UserProfile.processMapDatas[i].mapDetails[0].clearedstage == 25)
            {
                if (rewardStates[i] != MapProgressRewardItem.RewardState.CLAIMED)
                {
                    mapProgressRewardItems[i].Init(database.mapProgressRewards[i].iconImgs, MapProgressRewardItem.RewardState.AVAILABLE);
                    alarmIcon.SetActive(true);
                }

                sliderValue = i + 1;
            }
            else
            {
                mapProgressRewardItems[i].Init(database.mapProgressRewards[i].iconImgs, rewardStates[i]);
            }
        }

        slider.value = sliderValue;
        Slider.value = sliderValue;
        levelProgressTxt.text = sliderValue + " / 10";

        for (int i = 0; i < sliderValue; i++)
        {
            countBackgrounds[i].sprite = activeCountImg;
        }
    }

    public void ClaimReward(int index)
    {
        rewardStates[index] = MapProgressRewardItem.RewardState.CLAIMED;
        mapProgressRewardItems[index].SetState(MapProgressRewardItem.RewardState.CLAIMED);

        SoundManager.PlaySFX("Pickup_Gold_00");

        for (int i = 0; i < database.mapProgressRewards[index].rewards.Length; i++)
        {
            GameFlowManager.Instance.UserProfile.UpdateCurrency(database.mapProgressRewards[index].rewards[i], database.mapProgressRewards[index].rewardAmounts[i]);
        }
    }

    private void OnDestroy()
    {
        SaveGame.Save(Key, rewardStates);
    }
}
