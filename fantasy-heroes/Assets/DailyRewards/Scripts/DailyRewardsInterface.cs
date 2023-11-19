/***************************************************************************\
Project:      Daily Rewards
Copyright (c) Niobium Studios.
Author:       Guilherme Nunes Barbosa (gnunesb@gmail.com)
\***************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NiobiumStudios
{
    /**
     * The UI Logic Representation of the Daily Rewards
     **/
    public class DailyRewardsInterface : MonoBehaviour
    {
        [Header("Panel Reward")]
        public TMPro.TMP_Text textTimeDue;                    // Text showing how long until the next claim
        private bool readyToClaim;                  // Update flag
        [SerializeField] private List<DailyRewardUI> dailyRewardsUI = new List<DailyRewardUI>();
        [SerializeField] private DailyRewards dailyRewards;			// DailyReward Instance      
        [SerializeField] private GameObject claimableIcon;

        void Start()
        {
            InitializeDailyRewardsUI();

            UpdateUI();
        }

        void OnEnable()
        {
            dailyRewards.onClaimPrize += OnClaimPrize;
            dailyRewards.onInitialize += OnInitialize;
        }

        void OnDisable()
        {
            if (dailyRewards != null)
            {
                dailyRewards.onClaimPrize -= OnClaimPrize;
                dailyRewards.onInitialize -= OnInitialize;
            }
        }

        // Initializes the UI List based on the rewards size
        private void InitializeDailyRewardsUI()
        {
            for (int i = 0; i < dailyRewards.rewards.Count; i++)
            {
                int day = i + 1;
                var reward = dailyRewards.GetReward(day);

                dailyRewardsUI[i].Button.onClick.AddListener(() =>
                {
                    dailyRewards.ClaimPrize();
                    readyToClaim = false;
                    UpdateUI();
                });

                dailyRewardsUI[i].day = day;
                dailyRewardsUI[i].reward = reward;
                dailyRewardsUI[i].Initialize();
            }
        }

        public void UpdateUI()
        {
            dailyRewards.CheckRewards();

            bool isRewardAvailableNow = false;

            var lastReward = dailyRewards.lastReward;
            var availableReward = dailyRewards.availableReward;

            for (int i = 0; i < dailyRewardsUI.Count; i++)
            {
                var day = dailyRewardsUI[i].day;

                if (day == availableReward)
                {
                    dailyRewardsUI[i].state = DailyRewardUI.DailyRewardState.UNCLAIMED_AVAILABLE;

                    isRewardAvailableNow = true;
                }
                else if (day <= lastReward)
                {
                    dailyRewardsUI[i].state = DailyRewardUI.DailyRewardState.CLAIMED;
                }
                else if (day == lastReward + 1)
                {
                    dailyRewardsUI[i].state = DailyRewardUI.DailyRewardState.TOMORROW;
                }
                else
                {
                    dailyRewardsUI[i].state = DailyRewardUI.DailyRewardState.UNCLAIMED_UNAVAILABLE;
                }

                dailyRewardsUI[i].Refresh();
            }

            if (isRewardAvailableNow)
            {
                textTimeDue.text = "You can claim your reward!";
            }
            readyToClaim = isRewardAvailableNow;
        }


        void Update()
        {
            dailyRewards.TickTime();
            // Updates the time due
            CheckTimeDifference();
        }

        private void CheckTimeDifference()
        {
            if (!readyToClaim)
            {
                TimeSpan difference = dailyRewards.GetTimeDifference();

                // If the counter below 0 it means there is a new reward to claim
                if (difference.TotalSeconds <= 0)
                {
                    readyToClaim = true;
                    UpdateUI();
                }
                else
                {
                    string formattedTs = dailyRewards.GetFormattedTime(difference);

                    textTimeDue.text = string.Format(formattedTs);
                }
            }
        }

        // Delegate
        private void OnClaimPrize(int day)
        {
            var reward = dailyRewards.GetReward(day);
            claimableIcon.SetActive(false);//disable notify icon
            ClaimItemPopupManager.Instance.ShowTheClaimedItemWithAmount(reward.sprite, ItemType.GOLD, reward.reward);
        }

        private void OnInitialize(bool error, string errorMessage)
        {
            if (!error)
            {
                var showWhenNotAvailable = dailyRewards.keepOpen;
                var isRewardAvailable = dailyRewards.availableReward > 0;

                UpdateUI();
                if (showWhenNotAvailable || (!showWhenNotAvailable && isRewardAvailable))
                {
                    claimableIcon.SetActive(true);
                }
                else
                {
                    claimableIcon.SetActive(false);
                }

                CheckTimeDifference();
            }
        }
    }
}