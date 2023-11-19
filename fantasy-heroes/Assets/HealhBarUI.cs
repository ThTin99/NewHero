using System.Collections.Generic;
using UnityEngine;

public class HealhBarUI : MonoBehaviour
{
    public static HealhBarUI Instance { get; private set; }

    public GameObject HealthBarPrefab;
    Canvas m_Canvas;
    Queue<GameObject> m_Pool = new Queue<GameObject>();
    Camera m_MainCamera;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        m_Canvas = GetComponent<Canvas>();
        int POOL_SIZE = 50;
        for (int i = 0; i < POOL_SIZE; ++i)
        {
            var t = Instantiate(HealthBarPrefab, m_Canvas.transform);
            t.gameObject.SetActive(false);
            m_Pool.Enqueue(t);
        }
        m_MainCamera = Camera.main;
    }

    public void CreateNewHealthBar(GameObject obj)
    {
        var t = m_Pool.Dequeue();
        t.SetActive(true);
        HealthBarGO hbGO = t.GetComponent<HealthBarGO>();
        CharacterData charData = obj.GetComponent<CharacterData>();
        charData.healthBar = hbGO;
        charData.UpdateMaxHealth();
        hbGO.m_MainCamera = m_MainCamera;
        float distance = 2.5f;
        hbGO.distance = distance;
        hbGO.PosSetting(obj);
    }

    public void RetrieveHealthBar(GameObject bar)
    {
        m_Pool.Enqueue(bar);
    }
}

