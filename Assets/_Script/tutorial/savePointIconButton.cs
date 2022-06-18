using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class savePointIconButton : MonoBehaviour
{
    mapOpenSensor mapSensorScript;
    public int savePointNum;
    void Start()
    {
        mapSensorScript = GameObject.Find("mapOpenSensor").GetComponent<mapOpenSensor>();
    }

    
    void Update()
    {
        
    }

    public void Onclick()
    {
        mapSensorScript.selectedSavePointNum = savePointNum;
        mapSensorScript.iconCheck();
    }
}
