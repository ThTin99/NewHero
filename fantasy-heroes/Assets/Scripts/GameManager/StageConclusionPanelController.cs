using TMPro;
using UnityEngine;

public class StageConclusionPanelController : MonoBehaviour
{
    [SerializeField] private TMP_Text topFloorTxt;
    [SerializeField] private TMP_Text chapterTxt;
    [SerializeField] private Transform lootHolder;
    [SerializeField] private Sprite coinIcon;
    [SerializeField] private string lootItemKey = "LootItem";
    [SerializeField] private bool isVictory = false;
    [SerializeField] private Lean.Gui.LeanWindow thisPanel;

    private void OnEnable()
    {
        EventManager.StartListening(EventManager.GameEventEnum.ON_CLEARED_MAP, () => { if (isVictory) thisPanel.TurnOn(); });
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventManager.GameEventEnum.ON_CLEARED_MAP, () => { if (isVictory) thisPanel.TurnOn(); });
    }

    public void SpawnLootIcons()
    {
        //only spawm coin loot when coin amount > 0
        if (CoinManager.Instance.CurrentCoin > 0)
        {
            //Addressables.LoadAssetAsync<GameObject>(lootItemKey).Completed += OnLoadLootItemComplete;
        }
    }

    // private void OnLoadLootItemComplete(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj)
    // {
    //     if (obj.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Failed)
    //     {
    //         Invoke(nameof(SpawnLootIcons), 1f);
    //     }
    //     else
    //     {
    //         var item = obj.Result.GetComponent<LootItem>();
    //         item.UpdateDisplay(coinIcon, CoinManager.Instance.CurrentCoin);
    //         Instantiate(item, lootHolder);

    //         GameFlowManager.Instance.UserProfile.UpdateCurrency(ItemType.GOLD, (int)CoinManager.Instance.CurrentCoin);
    //         EventManager.TriggerEvent(EventManager.GameEventEnum.PICKUP_GOLD, (int)CoinManager.Instance.CurrentCoin);

    //         GameFlowManager.Instance.UserProfile.UpdateMapProgressData(GameFlowManager.Instance.ChosenMap, 0, GameManager.Instance.currentStage);

    //         if (isVictory)
    //         {
    //             EventManager.TriggerEvent(EventManager.GameEventEnum.CLEAR_MAP, 1);
    //             EventManager.TriggerEvent(EventManager.GameEventEnum.COMPLETE_CHAPTER, GameFlowManager.Instance.ChosenMap + 1);//map progress = index + 1
    //         }

    //         //Reward exp to player, can multiply with exp boost here
    //         GameFlowManager.Instance.UserProfile.RewardExp(GameFlowManager.Instance.ConsumedStaminaAmount);
    //     }
    // }

    public void UpdateStageInfoText()
    {
        topFloorTxt.text = "Top Floor " + GameManager.Instance.currentStage.ToString();
        chapterTxt.text = "Chapter " + GameFlowManager.Instance.ChosenMap + 1;
    }
}
