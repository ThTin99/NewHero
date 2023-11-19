using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScript : MonoBehaviour
{
    public UnityEngine.UI.Text StageTxt;
    void Start()
    {
        GameManager.Instance.SetupMapComplete();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStage(string stage){
        if(StageTxt != null)
        StageTxt.text = stage;
    }
}
