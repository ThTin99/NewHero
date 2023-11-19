using System;

[System.Serializable]
public class EquipmentSystem
{
    /// <summary>
    /// Handles the equipment stored inside an instance of CharacterData. Will take care of unequipping the previous
    /// item when equipping a new one in the same slot.
    /// </summary>

    public Action<EquipmentItem> OnEquiped { get; set; }
    public Action<EquipmentItem> OnUnequip { get; set; }

    CharacterData m_Owner;

    Weapon m_DefaultWeapon;

    public void Init(CharacterData owner, bool isHero = false)
    {
        m_Owner = owner;

        if (isHero)
        {
            GameFlowManager.Instance.UserProfile.Weapon?.EquippedBy(owner);
            GameFlowManager.Instance.UserProfile.m_HeadSlot?.EquippedBy(owner);
            GameFlowManager.Instance.UserProfile.m_TorsoSlot?.EquippedBy(owner);
            GameFlowManager.Instance.UserProfile.m_LegsSlot?.EquippedBy(owner);
            GameFlowManager.Instance.UserProfile.m_FeetSlot?.EquippedBy(owner);
            GameFlowManager.Instance.UserProfile.m_AccessorySlot?.EquippedBy(owner);

            if (GameFlowManager.Instance.UserProfile.Weapon == null)
            {
                Equip(m_DefaultWeapon);
            }
        }
    }

    public void InitWeapon(Weapon wep, CharacterData data)
    {
        m_DefaultWeapon = wep;
    }

    public EquipmentItem GetItem(EquipmentItem.EquipmentSlot slot)
    {
        switch (slot)
        {
            case EquipmentItem.EquipmentSlot.Head:
                return GameFlowManager.Instance.UserProfile.m_HeadSlot;
            case EquipmentItem.EquipmentSlot.Torso:
                return GameFlowManager.Instance.UserProfile.m_TorsoSlot;
            case EquipmentItem.EquipmentSlot.Legs:
                return GameFlowManager.Instance.UserProfile.m_LegsSlot;
            case EquipmentItem.EquipmentSlot.Pet1:
                return GameFlowManager.Instance.UserProfile.m_FeetSlot;
            case EquipmentItem.EquipmentSlot.Pet2:
                return GameFlowManager.Instance.UserProfile.m_AccessorySlot;
            default:
                return null;
        }
    }

    /// <summary>
    /// Equip the given item for the given user. This won't check about requirement, this should be done by the
    /// inventory before calling equip!
    /// </summary>
    /// <param name="item">Which item to equip</param>
    public void Equip(EquipmentItem item)
    {
        if (item == null)
        {
            return;
        }

        Unequip(item.Slot, true);

        OnEquiped?.Invoke(item);

        switch (item.Slot)
        {
            //special value for weapon
            case (EquipmentItem.EquipmentSlot)666:
                GameFlowManager.Instance.UserProfile.Weapon = item as Weapon;
                GameFlowManager.Instance.UserProfile.Weapon.EquippedBy(m_Owner);
                break;
            case EquipmentItem.EquipmentSlot.Head:
                {
                    GameFlowManager.Instance.UserProfile.m_HeadSlot = item;
                    GameFlowManager.Instance.UserProfile.m_HeadSlot.EquippedBy(m_Owner);
                }
                break;
            case EquipmentItem.EquipmentSlot.Torso:
                {
                    GameFlowManager.Instance.UserProfile.m_TorsoSlot = item;
                    GameFlowManager.Instance.UserProfile.m_TorsoSlot.EquippedBy(m_Owner);
                }
                break;
            case EquipmentItem.EquipmentSlot.Legs:
                {
                    GameFlowManager.Instance.UserProfile.m_LegsSlot = item;
                    GameFlowManager.Instance.UserProfile.m_LegsSlot.EquippedBy(m_Owner);
                }
                break;
            case EquipmentItem.EquipmentSlot.Pet1:
                {
                    GameFlowManager.Instance.UserProfile.m_FeetSlot = item;
                    GameFlowManager.Instance.UserProfile.m_FeetSlot.EquippedBy(m_Owner);
                }
                break;
            case EquipmentItem.EquipmentSlot.Pet2:
                {
                    GameFlowManager.Instance.UserProfile.m_AccessorySlot = item;
                    GameFlowManager.Instance.UserProfile.m_AccessorySlot.EquippedBy(m_Owner);
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Unequip the item in the given slot. isReplacement is used to tell the system if this unequip was called
    /// because we equipped something new in that slot or just unequip to empty slot. This is for the weapon : the
    /// weapon slot can't be empty, so if this is not a replacement, this will auto-requip the base weapon.
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="isReplacement"></param>
    public void Unequip(EquipmentItem.EquipmentSlot slot, bool isReplacement = false)
    {
        switch (slot)
        {
            case (EquipmentItem.EquipmentSlot)666:
                if (GameFlowManager.Instance.UserProfile.Weapon != null &&
                    (GameFlowManager.Instance.UserProfile.Weapon != m_DefaultWeapon || isReplacement)) // the only way to unequip the default weapon is through replacing it
                {
                    GameFlowManager.Instance.UserProfile.Weapon.UnequippedBy(m_Owner);

                    //the default weapon does not go back to the inventory
                    if (GameFlowManager.Instance.UserProfile.Weapon.ItemName != m_DefaultWeapon.ItemName)
                        GameFlowManager.Instance.InventorySystem.AddItem(GameFlowManager.Instance.UserProfile.Weapon);

                    OnUnequip?.Invoke(GameFlowManager.Instance.UserProfile.Weapon);
                    GameFlowManager.Instance.UserProfile.Weapon = null;

                    //reequip the default weapon if this is not an unequip to equip a new one
                    if (!isReplacement)
                        Equip(m_DefaultWeapon);
                }
                break;
            case EquipmentItem.EquipmentSlot.Head:
                if (GameFlowManager.Instance.UserProfile.m_HeadSlot != null)
                {
                    GameFlowManager.Instance.UserProfile.m_HeadSlot.UnequippedBy(m_Owner);
                    GameFlowManager.Instance.InventorySystem.AddItem(GameFlowManager.Instance.UserProfile.m_HeadSlot);
                    OnUnequip?.Invoke(GameFlowManager.Instance.UserProfile.m_HeadSlot);
                    GameFlowManager.Instance.UserProfile.m_HeadSlot = null;
                }
                break;
            case EquipmentItem.EquipmentSlot.Torso:
                if (GameFlowManager.Instance.UserProfile.m_TorsoSlot != null)
                {
                    GameFlowManager.Instance.UserProfile.m_TorsoSlot.UnequippedBy(m_Owner);
                    GameFlowManager.Instance.InventorySystem.AddItem(GameFlowManager.Instance.UserProfile.m_TorsoSlot);
                    OnUnequip?.Invoke(GameFlowManager.Instance.UserProfile.m_TorsoSlot);
                    GameFlowManager.Instance.UserProfile.m_TorsoSlot = null;
                }
                break;
            case EquipmentItem.EquipmentSlot.Legs:
                if (GameFlowManager.Instance.UserProfile.m_LegsSlot != null)
                {
                    GameFlowManager.Instance.UserProfile.m_LegsSlot.UnequippedBy(m_Owner);
                    GameFlowManager.Instance.InventorySystem.AddItem(GameFlowManager.Instance.UserProfile.m_LegsSlot);
                    OnUnequip?.Invoke(GameFlowManager.Instance.UserProfile.m_LegsSlot);
                    GameFlowManager.Instance.UserProfile.m_LegsSlot = null;
                }
                break;
            case EquipmentItem.EquipmentSlot.Pet1:
                if (GameFlowManager.Instance.UserProfile.m_FeetSlot != null)
                {
                    GameFlowManager.Instance.UserProfile.m_FeetSlot.UnequippedBy(m_Owner);
                    GameFlowManager.Instance.InventorySystem.AddItem(GameFlowManager.Instance.UserProfile.m_FeetSlot);
                    OnUnequip?.Invoke(GameFlowManager.Instance.UserProfile.m_FeetSlot);
                    GameFlowManager.Instance.UserProfile.m_FeetSlot = null;
                }
                break;
            case EquipmentItem.EquipmentSlot.Pet2:
                if (GameFlowManager.Instance.UserProfile.m_AccessorySlot != null)
                {
                    GameFlowManager.Instance.UserProfile.m_AccessorySlot.UnequippedBy(m_Owner);
                    GameFlowManager.Instance.InventorySystem.AddItem(GameFlowManager.Instance.UserProfile.m_AccessorySlot);
                    OnUnequip?.Invoke(GameFlowManager.Instance.UserProfile.m_AccessorySlot);
                    GameFlowManager.Instance.UserProfile.m_AccessorySlot = null;
                }
                break;
            default:
                break;
        }
    }
}
