using System.Collections.Generic;
using UnityEngine;

public class EquipmentScene : Singleton<EquipmentScene>
{
    public Equipment equipmentDatabase;

    public DataItem dataMaterial;

    public DataItem dataEquipment;

    public List<Transform> positionItem = new List<Transform>();

    public Transform materialParent;
    public Transform itemParent;
    public Transform UIContent;

    private void Start()
    {
        ShowMaterials();
        ShowEquipment();
    }

    public void ShowEquipment()
    {
        for (int i = 0; i < GameFlowManager.Instance.UserProfile.equipments.Count; i++)
        {
            if (GameFlowManager.Instance.UserProfile.equipments[i].On)
            {
                DataItem data = Instantiate(dataEquipment, positionItem[(int)GameFlowManager.Instance.UserProfile.equipments[i].Pos]);
                data.SetDataItem(GameFlowManager.Instance.UserProfile.equipments[i]);
            }
            else
            {
                DataItem data = Instantiate(dataEquipment, itemParent);
                data.SetDataItem(GameFlowManager.Instance.UserProfile.equipments[i]);
            }
        }
    }

    public void ShowMaterials()
    {
        for (int i = 0; i < GameFlowManager.Instance.UserProfile.materialValues.Count; i++)
        {
            DataItem data = Instantiate(dataMaterial, materialParent);
            data.SetDataMaterial(GameFlowManager.Instance.UserProfile.materialValues[i]);
        }
    }
}
