using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindButton : MonoBehaviour
{
    [SerializeField] WindHome windHome;
    [SerializeField] GameObject windZone;
    [SerializeField] GameObject wind;

    SpriteRenderer render;    
    public int windType;
    // 1이면 버튼으로 on/off 하는 환풍기
    // 2이면 타이머 내장 환풍기/ 평소에 꺼져있다가 버튼누르면 켜지는 방식 or 평소에 켜져있다가 버튼누르면 꺼지는 방식
    // 3이면 특정 주기에 따라 켜졌다 꺼졌다를 반복하는 환풍기 

    //type1
    bool isActive; //현재 windHome 이 작동하고 있는지의 여부 
    bool isButtonClicked; //현재 버튼이 눌려져 있는지의 여부 
    public float buttonClickDelay; //버튼이 눌려져 있는 시간 

    //type 2
    public bool isActDefault; //true 이면 켜져있는게 디폴트인 환풍기 
    public bool isTimerAct; //현재 타이머가 흘러가고 있는지의 여부 
    public float TimerActiveTime; //타이머 내장된 button이 작동하는 시간

    //type 3 : 이 경우는 버튼이 필요없이 환풍기가 알아서 작동하는 경우이므로, 버튼은 보이지 않는 곳에 숨겨두고 투명화시켜야 함 
    public float loopOffset; //맨 처음 시작하고 환풍기가 켜지기까지의 시간 
    public float loopDelay; //환풍기가 꺼지고 다시 켜지기까지의 시간
    public float loopActive; //환풍기가 켜져 있는 시간 

    BoxCollider2D windZoneColl;
    void Awake()
    {
        windZoneColl = windZone.GetComponent<BoxCollider2D>();

        if(windType == 1 && isActDefault)
        {
            windHome.enabled = true;
            windZoneColl.enabled = true;
            wind.SetActive(true);

            isActive = true;
        }
        else if (windType == 2 && isActDefault) //type 2 인데 켜진 상태가 default 인 환풍기일 경우
        {
            windHome.enabled = true;
            windZoneColl.enabled = true;
            wind.SetActive(true);

            isTimerAct = false;
        }
        else
        {
            windHome.enabled = false;
            windZoneColl.enabled = false;
            wind.SetActive(false);

            isActive = false;
            isTimerAct = false;
        }
    }

    void Start()
    {
        if(windType == 3)
        {
            StartCoroutine(type3_windAct());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isButtonClicked && windType == 1) return;
        //type1: 버튼이 다시 올라오기 전이면 작동x
        if (isTimerAct && windType == 2) return;
        //type2: 타이머 시간이 흘러가는 중에는 버튼 눌러도 반응x 

        if(collision.gameObject.tag == "Player" && collision.transform.up == transform.up)
        {
            if(windType == 1)
            {
                type1_windAct();
            }            
            else if(windType == 2)
            {
                type2_windAct();
            }
        }
    }

    void type1_windAct() //작동하고 있던 환풍기는 끄고 꺼져 있던 환풍기는 키기 
    {
        if (!isActive) //꺼져 있을땐 다시 켜야 함 
        {
            windHome.enabled = true;
            windZoneColl.enabled = true;
            wind.SetActive(true);

            isActive = true;
            buttonClick(); //버튼이 눌림 
            Invoke("buttonReturn", buttonClickDelay);
        }
        else
        {
            windHome.enabled = false;
            windZoneColl.enabled = false;
            wind.SetActive(false);

            isActive = false;
            buttonClick(); //버튼이 눌림 
            Invoke("buttonReturn", buttonClickDelay);
        }        
    }

    void type2_windAct()
    {
        if (isActDefault)
        {
            windHome.enabled = false;
            windZoneColl.enabled = false;
            wind.SetActive(false);

            isTimerAct = true;
            buttonClick(); //버튼이 눌림 

            Invoke("type2_windOn", TimerActiveTime); //설정된 시간이 흐른 뒤 환풍기 저절로 끔 
            Invoke("buttonReturn", buttonClickDelay);
        }
        else
        {
            windHome.enabled = true;
            windZoneColl.enabled = true;
            wind.SetActive(true);

            isTimerAct = true;
            buttonClick(); //버튼이 눌림 

            Invoke("type2_windOff", TimerActiveTime); //설정된 시간이 흐른 뒤 환풍기 저절로 끔 
            Invoke("buttonReturn", buttonClickDelay);
        }      
    }

    void type2_windOn()
    {
        windHome.enabled = true;
        windZoneColl.enabled = true;
        wind.SetActive(true);

        isTimerAct = false;
    }
    void type2_windOff()
    {
        windHome.enabled = false;
        windZoneColl.enabled = false;
        wind.SetActive(false);

        isTimerAct = false;
    }

    IEnumerator type3_windAct()
    {
        yield return new WaitForSeconds(loopOffset);

        while (true)
        {
            //환풍기 켜기 
            windHome.enabled = true;
            windZoneColl.enabled = true;
            wind.SetActive(true);

            yield return new WaitForSeconds(loopActive);

            windHome.enabled = false;
            windZoneColl.enabled = false;
            wind.SetActive(false);

            yield return new WaitForSeconds(loopDelay);
        }
    }

    void buttonClick()
    {
        transform.position -= transform.up * 0.2f;
        isButtonClicked = true;
    }
    void buttonReturn()
    {
        transform.position += transform.up * 0.2f;
        isButtonClicked = false;
    }
}