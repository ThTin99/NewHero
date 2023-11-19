/***************************************************************************\
Project:      Daily Rewards
Copyright (c) Niobium Studios.
Author:       Guilherme Nunes Barbosa (gnunesb@gmail.com)
\***************************************************************************/
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace NiobiumStudios
{
    /**
    * Daily Rewards keeps track of the player daily rewards based on the time he last selected a reward
    **/
    public class DailyRewards : DailyRewardsCore<DailyRewards>
    {
        public List<Reward> rewards;        // Rewards list 
        public DateTime lastRewardTime;     // The last time the user clicked in a reward
        public int availableReward;         // The available reward position the player claim
        public int lastReward;              // the last reward the player claimed
        public bool keepOpen = true;        // Keep open even when there are no Rewards available?

        // Delegates
        public delegate void OnClaimPrize(int day);                 // When the player claims the prize
        public OnClaimPrize onClaimPrize;

        // Needed Constants
        private const string LAST_REWARD_TIME = "LastRewardTime";
        private const string LAST_REWARD = "LastReward";
        private const string DEBUG_TIME = "DebugTime";
        private const string FMT = "O";

        public TimeSpan debugTime;         // For debug purposes only

        void Start()
        {
            // Initializes the timer with the current time
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            base.InitializeDate();

            LoadDebugTime();
            CheckRewards();

            onInitialize?.Invoke();
        }

        protected override void OnApplicationPause(bool pauseStatus)
        {
            base.OnApplicationPause(pauseStatus);
            CheckRewards();
        }

        public TimeSpan GetTimeDifference()
        {
            var tomorrow = now.AddDays(1).AddHours(-now.Hour).AddSeconds(-now.Second);
            return tomorrow.Subtract(now);
        }

        private void LoadDebugTime()
        {
            int debugHours = PlayerPrefs.GetInt(GetDebugTimeKey(), 0);
            debugTime = new TimeSpan(debugHours, 0, 0);
        }

        // Check if the player have unclaimed prizes
        public void CheckRewards()
        {
            if (!isInitialized)
            {
                return;
            }

            string lastClaimedTimeStr = PlayerPrefs.GetString(GetLastRewardTimeKey());
            lastReward = PlayerPrefs.GetInt(GetLastRewardKey());

            // It is not the first time the user claimed.
            // We need to know if he can claim another reward or not
            if (!string.IsNullOrEmpty(lastClaimedTimeStr))
            {
                lastRewardTime = DateTime.ParseExact(lastClaimedTimeStr, FMT, CultureInfo.InvariantCulture);

                if (lastRewardTime.Date == now.Date)
                {
                    // No claim for you. Try tomorrow
                    availableReward = 0;
                    return;
                }

                // The player can only claim if he logs between the following day and the next.
                if (lastRewardTime.Date == now.Date.AddDays(-1f))
                {
                    // If reached the last reward, resets to the first restarting the cicle
                    if (lastReward == rewards.Count)
                    {
                        availableReward = 1;
                        lastReward = 0;
                        return;
                    }

                    availableReward = lastReward + 1;

                    Debug.Log(" Player can claim prize " + availableReward);
                    return;
                }

                if (lastRewardTime.Date <= now.Date.AddDays(-2f))
                {
                    // The player loses the following day reward and resets the prize
                    availableReward = 1;
                    lastReward = 0;
                    Debug.Log(" Prize reset ");
                }
            }
            else
            {
                // Is this the first time? Shows only the first reward
                availableReward = 1;
            }
        }

        // Checks if the player claim the prize and claims it by calling the delegate. Avoids duplicate call
        public void ClaimPrize()
        {
            if (availableReward > 0)
            {
                SoundManager.PlaySFX("Pickup_Gold_00");

                // Delegate
                onClaimPrize?.Invoke(availableReward);

                Debug.Log(" Reward [" + rewards[availableReward - 1] + "] Claimed!");
                PlayerPrefs.SetInt(GetLastRewardKey(), availableReward);

                string lastClaimedStr = now.AddHours(debugTime.TotalHours).ToString(FMT);
                PlayerPrefs.SetString(GetLastRewardTimeKey(), lastClaimedStr);
                PlayerPrefs.SetInt(GetDebugTimeKey(), (int)debugTime.TotalHours);
            }
            else
            {
                SoundManager.PlaySFX("Button");
            }

            CheckRewards();
        }

        //Returns the lastReward playerPrefs key depending on instanceId
        private string GetLastRewardKey()
        {
            return LAST_REWARD;
        }

        //Returns the lastRewardTime playerPrefs key depending on instanceId
        private string GetLastRewardTimeKey()
        {
            return LAST_REWARD_TIME;
        }

        //Returns the advanced debug time playerPrefs key depending on instanceId
        private string GetDebugTimeKey()
        {
            return DEBUG_TIME;
        }

        // Returns the daily Reward of the day
        public Reward GetReward(int day)
        {
            return rewards[day - 1];
        }
    }
}