using System.Collections.Generic;
using UnityEngine;

public class NPCSensor_Base : MonoBehaviour
{
    public Enemy npcBase;

    protected List<GameObject> sensedObjects = new List<GameObject>();

    void Start()
    {
        if (npcBase == null)
        {
            npcBase = gameObject.GetComponent<Enemy>();
        }

        StartSensor();
    }

    void Update()
    {
        UpdateSensor();
    }

    protected virtual void StartSensor() { }

    protected virtual void UpdateSensor() { }

    protected List<GameObject> GetSensedObjects()
    {
        return sensedObjects;
    }
}
