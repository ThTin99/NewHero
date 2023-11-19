using DG.Tweening;
using Lean.Pool;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : Singleton<CoinManager>
{
    [SerializeField] private GameObject coinPrefabAsset;
    [SerializeField] private float minAnimDuration;
    [SerializeField] private float maxAnimDuration;
    [SerializeField] private Ease easeType;
    [Header("Coin Counter")]
    [SerializeField] private TMPro.TMP_Text coinText;

    private List<GameObject> coinsOnStage = new List<GameObject>();
    private GameObject player;
    private uint currentCoin;

    private void OnEnable()
    {
        EventManager.StartListening(EventManager.GameEventEnum.ON_CLEARED_STAGE, CollectAllCoinsOnStage);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventManager.GameEventEnum.ON_CLEARED_STAGE, CollectAllCoinsOnStage);
    }

    public uint CurrentCoin { get => currentCoin; set => currentCoin = value; }

    internal void CoinManagerSetup()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void SpawnSomeCoinsAtPosition(Vector3 pos)
    {
        coinsOnStage.Add(LeanPool.Spawn(coinPrefabAsset, pos, Quaternion.identity));
    }

    public void CollectAllCoinsOnStage()
    {
        foreach (var coin in coinsOnStage)
        {
            //animate coin to target position
            float duration = Random.Range(minAnimDuration, maxAnimDuration);
            coin.transform.DOMove(player.transform.position, duration)
            .SetEase(easeType)
            .OnComplete(() =>
            {
                LeanPool.Despawn(coin);
                AddCoin(1);
            });
        }
        coinsOnStage.Clear();
    }

    public void UpdateCoinDisplay()
    {
        coinText.text = currentCoin.ToString();
    }

    public void AddCoin(uint amount)
    {
        currentCoin += amount;
        UpdateCoinDisplay();
    }
}
