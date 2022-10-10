using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floorDoorSensor : MonoBehaviour
{
    //센서 위에 플레이어 or Box가 올라가면 센서가 인식해서 문을 열어줌

    [SerializeField] bool isPlayerOn;
    [SerializeField] bool isObjectOn;

    public GameObject doorCol;
    SpriteRenderer spr;
    BoxCollider2D doorBox;

    public Sprite[] doorSpr_idle;
    public Sprite[] doorSpr_active;

    IEnumerator curCoroutine; //한 번에 하나의 코루틴만 작동하도록 함 

    bool isDoorActing = false;

    private void Awake()
    {
        spr = doorCol.GetComponent<SpriteRenderer>();
        doorBox = doorCol.GetComponent<BoxCollider2D>();
    }

    void Start()
    { 
        spr.sprite = doorSpr_idle[0];

        if (curCoroutine != null)
        {
            StopCoroutine(curCoroutine);
        }

        curCoroutine = doorIdle();
        StartCoroutine(curCoroutine); //시작하면 기본 애니메이션 실행
    }
    
    void Update()
    {
        if(!isDoorActing)
        {
            curCoroutine = doorIdle();
            StartCoroutine(curCoroutine);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            isPlayerOn = true;
            if (curCoroutine != doorOpen())
            {
                StopCoroutine(curCoroutine);
                curCoroutine = doorOpen();

                StartCoroutine(curCoroutine);
            }
        }
        if(collision.tag == "Platform")
        {
            isObjectOn = true;
            if (curCoroutine != doorOpen())
            {
                StopCoroutine(curCoroutine);
                curCoroutine = doorOpen();

                StartCoroutine(curCoroutine);
            }
        }   
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isPlayerOn = false;
            if (!isObjectOn) //센서 위에 아무것도 없는 경우 ~> 문 닫음 
            {
                StopCoroutine(curCoroutine);
                curCoroutine = doorClose();

                StartCoroutine(curCoroutine);
            }
        }
        else if (collision.tag == "Platform")
        {
            isObjectOn = false;
            if (!isPlayerOn) //센서 위에 아무것도 없는 경우 ~> 문 닫음 
            {
                StopCoroutine(curCoroutine);
                curCoroutine = doorClose();

                StartCoroutine(curCoroutine);
            }
        }
    }
   
    IEnumerator doorIdle() //가만히 있을 때 작동하는 애니메이션
    {
        isDoorActing = true;

        while (true)
        {
            for (int index = 0; index < doorSpr_idle.Length; index++)
            {
                spr.sprite = doorSpr_idle[index];
                yield return new WaitForSeconds(0.08f);
            }
        }
    }

    IEnumerator doorOpen()
    {
        isDoorActing = true;

        for (int index = 0; index < doorSpr_active.Length; index++)
        {
            spr.sprite = doorSpr_active[index];
            yield return new WaitForSeconds(0.05f);
        }

        spr.enabled = false; //문 여는 애니메이션 끝나면 아예 안 보이게 함 
        doorBox.enabled = false;

        isDoorActing = false;
    }

    IEnumerator doorClose()
    {
        isDoorActing = true;
        
        spr.enabled = true;
        doorBox.enabled = true;

        for (int index = doorSpr_active.Length-1; index >= 0; index--)
        {
            spr.sprite = doorSpr_active[index];
            yield return new WaitForSeconds(0.05f);
        }

        isDoorActing = false; 
    }
}
