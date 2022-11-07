using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class electricSensor : MonoBehaviour
{
    public bool magWork; //true �� ���¿��� platform �� �ݶ��̴��� ������ ������Ŵ    
    public Sprite[] spriteGroup;
    //[0]: �� ���� [1]: �� ���� 

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
