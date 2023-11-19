/***************************************************************************\
Project:      Daily Rewards
Copyright (c) Niobium Studios.
Author:       Guilherme Nunes Barbosa (gnunesb@gmail.com)
\***************************************************************************/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/* 
 * Daily Reward Object UI representation
 */
namespace NiobiumStudios
{
    /** 
     * The UI Representation of a Daily Reward.
     * 
     *  There are 3 states:
     *  
     *  1. Unclaimed and available:
     *  - Shows focus UI and Interactable btn
     *  
     *  2. Unclaimed and Unavailable
     *  - Uninteractable btn
     *  
     *  3. Claimed
     *  - Shows clear UI and Uninteractable btn
     *  
     *  4. Tomorrow
     *  - Shows focus & tomorrow UI and Uninteractable btn
     **/
    public class DailyRewardUI : MonoBehaviour
    {
        public bool showRewardName;

        [Header("UI Elements")]
        public TMP_Text textDay;                // Text containing the Day text eg. Day 12
        public TMP_Text textReward;             // The Text containing the Reward amount
        public Image imageReward;           // The Reward Image

        [SerializeField] private GameObject clearUI;
        [SerializeField] private GameObject focusUI;
        [SerializeField] private GameObject tomorrowUI;
        [SerializeField] private Button button;

        [Header("Internal")]
        public int day;

        [HideInInspector]
        public Reward reward;

        public DailyRewardState state;

        public Button Button { get => button; set => button = value; }

        // The States a reward can have
        public enum DailyRewardState
        {
            UNCLAIMED_AVAILABLE,
            UNCLAIMED_UNAVAILABLE,
            CLAIMED,
            TOMORROW
        }

        public void Initialize()
        {
            textDay.text = "Day " + day.ToString();
            if (reward.reward > 0)
            {
                if (showRewardName)
                    textReward.text = reward.unit + "x" + reward.reward;
                else
                    textReward.text = reward.reward.ToString();
            }
            // else
            // {
            //     textReward.text = reward.unit.ToString();
            // }
            imageReward.sprite = reward.sprite;
        }

        // Refreshes the UI
        public void Refresh()
        {
            switch (state)
            {
                case DailyRewardState.UNCLAIMED_AVAILABLE:
                    clearUI.SetActive(false);
                    focusUI.SetActive(true);
                    tomorrowUI.SetActive(false);
                    break;
                case DailyRewardState.UNCLAIMED_UNAVAILABLE:
                    clearUI.SetActive(false);
                    tomorrowUI.SetActive(false);
                    focusUI.SetActive(false);
                    break;
                case DailyRewardState.CLAIMED:
                    clearUI.SetActive(true);
                    focusUI.SetActive(false);
                    tomorrowUI.SetActive(false);
                    break;
                case DailyRewardState.TOMORROW:
                    clearUI.SetActive(false);
                    focusUI.SetActive(true);
                    tomorrowUI.SetActive(true);
                    break;
            }
        }
    }
}