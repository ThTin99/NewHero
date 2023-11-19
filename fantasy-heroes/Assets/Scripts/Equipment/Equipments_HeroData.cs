using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroData : MonoBehaviour
{
    public static HeroData herodata;
    public static int attack, armor,equipmentlv,speedattack;
    public static string nameItem;
    public static Image img;

    public Equipment eqm;
    private void Awake()
    {
        herodata = this;
    }

    public void GetEquipmentinfor()
    {
        
    }

}
