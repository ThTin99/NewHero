﻿using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    public ItemEntryUI HeadSlot;
    public ItemEntryUI TorsoSlot;
    public ItemEntryUI LegsSlot;
    public ItemEntryUI FeetSlot;
    public ItemEntryUI AccessorySlot;
    public ItemEntryUI WeaponSlot;

    public void Init(InventoryUI owner)
    {
        HeadSlot.Owner = owner;
        TorsoSlot.Owner = owner;
        LegsSlot.Owner = owner;
        FeetSlot.Owner = owner;
        AccessorySlot.Owner = owner;
        WeaponSlot.Owner = owner;
    }

    public void UpdateEquipment(EquipmentSystem equipment, StatSystem system)
    {
        var head = equipment.GetItem(EquipmentItem.EquipmentSlot.Head);
        var torso = equipment.GetItem(EquipmentItem.EquipmentSlot.Torso);
        var legs = equipment.GetItem(EquipmentItem.EquipmentSlot.Legs);
        var feet = equipment.GetItem(EquipmentItem.EquipmentSlot.Pet1);
        var accessory = equipment.GetItem(EquipmentItem.EquipmentSlot.Pet2);
        var weapon = GameFlowManager.Instance.UserProfile.Weapon;

        HeadSlot.SetupEquipment(head);
        TorsoSlot.SetupEquipment(torso);
        LegsSlot.SetupEquipment(legs);
        FeetSlot.SetupEquipment(feet);
        AccessorySlot.SetupEquipment(accessory);
        WeaponSlot.SetupEquipment(weapon);
    }
}
