using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindButton : MonoBehaviour
{
    [SerializeField] WindHome windHome;
    [SerializeField] GameObject windZone;
    [SerializeField] GameObject wind;

    public Sprite[] leverSprite;
    //[0]은 왼쪽, [1]는 오른쪽으로 기울어진 레버 
  
    public int windType;
    // 1이면 레버로 on/off 하는 환풍기
    // on 상태면 레버는 sprite[0], off 상태면 레버는 sprite[1] 로 좌/우를 왔다갔다해야 함 

    // 2이면 타이머 내장 환풍기/ 평소에 꺼져있다가 버튼누르면 켜지는 방식 or 평소에 켜져있다가 버튼누르면 꺼지는 방식
    // on 상태면 레버는 sprite[0], off 상태면 레버는 sprite[1], 타이머가 다 되면 레버 스프라이트도 자동으로 바뀌어야 함 

    // 3이면 특정 주기에 따라 켜졌다 꺼졌다를 반복하는 환풍기 
    // 레버의 spr 은 끄고 보이지 않게 숨겨둬야 함 

    //type1
    bool isActive; //현재 windHome 이 작동하고 있는지의 여부     

    //type 2
    public bool isTimerAct; //현재 타이머가 흘러가고 있는지의 여부 
    public float TimerActiveTime; //타이머 내장된 button이 작동하는 시간

    //type1 && type2 
    public bool isActDefault; //true 이면 켜져있는게 디폴트인 환풍기 

    //type 3 : 이 경우는 버튼이 필요없이 환풍기가 알아서 작동하는 경우이므로, 버튼은 보이지 않는 곳에 숨겨두고 투명화시켜야 함 
    public float loopOffset; //맨 처음 시작하고 환풍기가 켜지기까지의 시간 
    public float loopDelay; //환풍기가 꺼지고 다시 켜지기까지의 시간
    public float loopActive; //환풍기가 켜져 있는 시간 

    BoxCollider2D windZoneColl;
    SpriteRenderer spr;

    bool isPlayerOn= false;

    AudioSource sound;
    void Awake()
    {
        windZoneColl = windZone.GetComponent<BoxCollider2D>();
        spr = GetComponent<SpriteRenderer>();
        sound = GetComponent<AudioSource>();

        //환풍기의 초기상태 설정 

        if(isActDefault && (windType == 1 || windType == 2)) 
            //환풍기가 type1 or type2 이면서 켜진 상태가 디폴트이면 환풍기는 처음부터 켜져야 함 
        {
            windHome.enabled = true;
            windZoneColl.enabled = true;
            wind.SetActive(true);

            isActive = true;
            spr.sprite = leverSprite[1]; 
        }
        else
            //그 밖의 경우는 꺼 둠 
        {
            windHome.enabled = false;
            windZoneColl.enabled = false;
            wind.SetActive(false);

            isActive = false;
            isTimerAct = false;
            spr.sprite = leverSprite[0];
        }
    }

    void Start()
    {
        if(windType == 3) //type3이면 플레이어가 상호작용 할 필요 없이 바로 코루틴 시작함 
        {
            StartCoroutine(type3_windAct());
        }
    }
    private void Update()
    {
        if (windType == 2 && isTimerAct) return;
        //type2 환풍기의 경우 타이머 작동중이면 플레이어 동작 무시 

        if(isPlayerOn && Input.GetKeyDown(KeyCode.E))
        {
            if (windType == 1)
            {
                type1_windAct();

                sound.Stop();
                sound.Play();
            }
            else if (windType == 2)
            {
                StartCoroutine(type2_windAct());

                sound.Stop();
                sound.Play();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && collision.transform.up == transform.up)
        {
            isPlayerOn = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && collision.transform.up == transform.up)
        {
            isPlayerOn = false;
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
            spr.sprite = leverSprite[1]; //레버 킴 
        }
        else
        {
            windHome.enabled = false;
            windZoneColl.enabled = false;
            wind.SetActive(false);

            isActive = false;
            spr.sprite = leverSprite[0]; //레버 끔 
        }        
    }

    IEnumerator type2_windAct() //레버 당기면 타이머가 시작, 타이머 끝나면 다시 원래대로 돌아옴 
    {
        if (isActDefault) //켜져있는 상태가 디폴트이면 
        {
            windHome.enabled = false;
            windZoneColl.enabled = false;
            wind.SetActive(false);
            isTimerAct = true;
            spr.sprite = leverSprite[1];

            yield return new WaitForSeconds(TimerActiveTime);

            windHome.enabled = true;
            windZoneColl.enabled = true;
            wind.SetActive(true);
            isTimerAct = false;
            spr.sprite = leverSprite[0];
        }
        else
        {
            windHome.enabled = true;
            windZoneColl.enabled = true;
            wind.SetActive(true);
            isTimerAct = true;
            spr.sprite = leverSprite[0];

            yield return new WaitForSeconds(TimerActiveTime);

            windHome.enabled = false;
            windZoneColl.enabled = false;
            wind.SetActive(false);
            isTimerAct = false;
            spr.sprite = leverSprite[1];
        }      
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
}