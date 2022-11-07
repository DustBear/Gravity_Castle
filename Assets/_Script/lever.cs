using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lever : MonoBehaviour
{
    Sprite lightOff; //불 꺼진 모습 
    SpriteRenderer spr;
    GameObject playerObj;

    public Sprite[] spriteGroup; //[0] ~[5]는 default, [6]은 불 꺼짐 

    public bool isPowerLever; //true 이면 180도 회전하는 레버가 됨 
    bool shouldLightOn = true;
    
    private void Awake()
    {
        playerObj = GameObject.Find("Player");
        spr = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        StartCoroutine(default_anim());
    }
    
    public void lightTurnOff()
    {
        shouldLightOn = false;
        spr.sprite = spriteGroup[spriteGroup.Length-1];
    }
    public void lightTurnOn()
    {
        shouldLightOn = true;
        StartCoroutine(default_anim());
    }

    IEnumerator default_anim()
    {
        var frame= new WaitForSeconds(0.14f);
        while (true)
        {
            for(int index=0; index<spriteGroup.Length-1; index++)
            {
                if (!shouldLightOn)
                {
                    yield break;
                }

                spr.sprite = spriteGroup[index];
                yield return frame;
            }
        }
    }
}
