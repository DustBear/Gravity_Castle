using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class savePointIconButton : MonoBehaviour
{
    stageManager stageManagerScr;
    public int savePointNum;
    void Start()
    {
        stageManagerScr = GameObject.Find("stageManager").GetComponent<stageManager>();
    }

    
    void Update()
    {
        
    }

    public void Onclick()
    {
        UIManager.instance.clickSoundGen();

        stageManagerScr.selectedSavePointNum = savePointNum;
        stageManagerScr.iconCheck();
    }
}
