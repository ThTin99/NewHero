using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Data", menuName = "Shop Data", order = 52)]
public class ShopData : ScriptableObject
{
    public List<DailyDealData> goldDailyDeals;
    public List<DailyDealData> diamondDailyDeals;
}

[System.Serializable]
public struct DailyDealData
{
    public int amount;
    public ItemType ItemType;
    public Sprite icon;
    public float price;
    public ItemType priceType;
    public bool isBestChoice;
    public string iapString;
}