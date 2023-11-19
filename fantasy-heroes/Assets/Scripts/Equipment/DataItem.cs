using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataItem : MonoBehaviour
{
    public string nameItem, nameMate;
    public TextMeshProUGUI amount, LevelItem;
    public int attack, health, armor, speed, crit, type;

    public InfoItem inforpopup;

    public List<Sprite> FrameEquipment = new List<Sprite>();

    public Image imgEquipment, imgMaterial, BGItem;

    public void SetDataItem(EquipmentValue value)
    {
        var valueFromDb = EquipmentScene.Instance.equipmentDatabase.listEquipment.Find(x => x.EquipmentId == value.EquipmentId);
        attack = valueFromDb.attack;
        health = valueFromDb.health;
        armor = valueFromDb.armor;
        speed = valueFromDb.attackSpeed;
        crit = valueFromDb.critdmg;
        nameItem = valueFromDb.EquipmentId.ToString();
        LevelItem.text = value.Lv.ToString();
        armor = valueFromDb.armor;
        imgEquipment.sprite = valueFromDb.Img;
        type = (int)valueFromDb.Rarity;

        switch (type)
        {
            case 1:
                BGItem.sprite = FrameEquipment[0];
                break;
            case 2:
                BGItem.sprite = FrameEquipment[1];
                break;
            case 3:
                BGItem.sprite = FrameEquipment[2];
                break;
            case 4:
                BGItem.sprite = FrameEquipment[3];
                break;
        }
    }

    public void SetDataMaterial(MaterialsValue value)
    {
        var valueFromDb = EquipmentScene.Instance.equipmentDatabase.ListMaterial.Find(x => x.MaterialId == value.MaterialId);
        nameMate = valueFromDb.Namematerial;
        amount.text = value.Amount.ToString();
        imgMaterial.sprite = valueFromDb.ImgMaterial;
    }

    public void ShowPopupItem()
    {
        InfoItem info = Instantiate(inforpopup, EquipmentScene.Instance.UIContent);
        info.SetInfor(this);
    }
}
