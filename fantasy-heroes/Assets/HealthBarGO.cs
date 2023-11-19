using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarGO : MonoBehaviour
{
    public GameObject target;
    public Camera m_MainCamera;
    public Slider slider;
    [SerializeField] TextMeshProUGUI value;
    public float distance;

    private void Start() {
        m_MainCamera = Camera.main;
    }

    private void Update() {
        if(target != null && gameObject.activeSelf){
            PosSetting(target);
        }
    }

    public void PosSetting(GameObject obj){

        target = obj;
        Vector3 WorldPositionStart = target.transform.position + new Vector3(0f,0f,distance) + Vector3.up;
        Vector3 pos = m_MainCamera.WorldToScreenPoint(WorldPositionStart);
        pos.z = 0.0f;
        transform.position = pos;
    }

    public void HealthSetting(int max, int current){
        slider.value = (float)current/max;
        if(value != null) value.text = current.ToString();
    }

    public void EnergySetting(float max, float current){
        slider.value = (float)current/max;
    }
}
