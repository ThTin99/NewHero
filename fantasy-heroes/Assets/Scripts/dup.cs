using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dup : MonoBehaviour
{
       public TMPro.TMP_Text abilityName1;
       public TMPro.TMP_Text abilityName2;

    public void Update()
    {
        abilityName1.text = abilityName2.text;
    }
}
