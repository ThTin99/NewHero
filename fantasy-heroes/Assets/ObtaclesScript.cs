using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObtaclesScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.CreateObstaclesComplete();
    }

    public void Destroy(){
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
