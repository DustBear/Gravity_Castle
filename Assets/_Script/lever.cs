using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lever : MonoBehaviour
{
    Sprite lightOff; //불 꺼진 모습 
    Animator ani;
    GameObject playerObj;

    public bool isPowerLever; //true �̸� 180�� ȸ�������� ����, false�� 90���� ȸ�������� ���� 

    private void Awake()
    {
        ani = GetComponent<Animator>();
        playerObj = GameObject.Find("Player");

        lightTurnOn();
    }
    private void Start()
    {
        
    }
    
    public void lightTurnOff()
    {
        if (!isPowerLever)
        {
            ani.Play("lever_90_default");
        }
        else
        {
            ani.Play("lever_180_default");
        }       
    }
    public void lightTurnOn()
    {
        if (!isPowerLever)
        {
            ani.Play("lever_90");
        }
        else
        {
            ani.Play("lever_180");
        }
    }
}
