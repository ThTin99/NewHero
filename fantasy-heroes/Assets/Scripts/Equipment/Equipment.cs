using System;
using System.Collections.Generic;
using UnityEngine;

public enum Rarity { Normal, Rare, Unique, Legendary }
public enum Position { Weapon, Shield, Armor, gloves, Shoes, Pet }
public enum EquipmentId { BigBoy, Turtle, DoubleBlade }
public enum MaterialId { SwordUpdate, ShieldUpdate }

[CreateAssetMenu(fileName = "New Equipment", menuName = "Equipment")]
public class Equipment : ScriptableObject
{
    public List<EquipmentValue> listEquipment = new List<EquipmentValue>();
    public List<MaterialsValue> ListMaterial = new List<MaterialsValue>();
}

[System.Serializable]
public class EquipmentValue
{
    public EquipmentId EquipmentId;
    public string name;
    public Sprite Img;
    public int type;

    public Rarity Rarity;
    public Position Pos;

    public bool On;
    public int Lv;
    public int attack;
    public int health;
    public int armor;
    public int attackSpeed;
    public int critdmg;

    //only use this to add equipment to userdata
    public EquipmentValue(EquipmentId equipmentId, bool on, int lv)
    {
        EquipmentId = equipmentId;
        On = on;
        Lv = lv;
    }
}

[System.Serializable]
public class MaterialsValue
{
    public MaterialId MaterialId;
    public string Namematerial;
    public int Amount = 10;
    public Sprite ImgMaterial;

    //only use this to add material to userdata
    public MaterialsValue(MaterialId materialId, int amount)
    {
        MaterialId = materialId;
        Amount = amount;
    }
}
