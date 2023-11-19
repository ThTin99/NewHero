using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadPlayerPoint : MonoBehaviour
{
    Vector3 startpoint;
        
    public GameObject player;
    public GameObject Map;
    public MapData MapData;

    public void Start()
    {
        startpoint = player.transform.position;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            player.transform.position = startpoint;
            Instantiate(Map, new Vector3(0,0,0), Quaternion.identity);
        }
    }
}
