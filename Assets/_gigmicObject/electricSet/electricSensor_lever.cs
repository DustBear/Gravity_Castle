using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class electricSensor_lever : MonoBehaviour
{
    public Sprite[] leverSprite;
    
    public GameObject magPlatform; //�ڼ� �ۿ��ϴ� �÷��� 
    SpriteRenderer spr;
    SpriteRenderer sensorSpr;

    bool isPlayerOn;
    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        sensorSpr = magPlatform.GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerOn && Input.GetKeyDown(KeyCode.E))
        {
            magPlatform.GetComponent<electricSensor>().magWork = !magPlatform.GetComponent<electricSensor>().magWork;
            //true �� false ��, false �̸� true �� �ٲ��� 

            if (magPlatform.GetComponent<electricSensor>().magWork)
            {
                spr.sprite = leverSprite[1];
            }
            else
            {
                spr.sprite = leverSprite[0];
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && collision.transform.up == transform.up)
        {
            isPlayerOn = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision.transform.up == transform.up)
        {
            isPlayerOn = false;
        }
    }

}
