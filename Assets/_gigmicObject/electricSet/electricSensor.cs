using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class electricSensor : MonoBehaviour
{
    public bool magWork; //true 인 상태에서 platform 이 콜라이더에 닿으면 정지시킴    
    public Sprite[] spriteGroup;
    //[0]: 불 꺼짐 [1]: 불 켜짐 

    SpriteRenderer spr;

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        
    }

    
    void Update()
    {
        if (magWork)
        {
            if(spr.sprite != spriteGroup[1])
            {
                spr.sprite = spriteGroup[1];
            }
        }
        else
        {
            if (spr.sprite != spriteGroup[0])
            {
                spr.sprite = spriteGroup[0];
            }
        }
    }

}
