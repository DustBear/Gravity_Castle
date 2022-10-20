using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elecShock : MonoBehaviour
{
    public GameObject electric; //통제할 전기 오브젝트 
    public int elecType;
    public Sprite[] leverSprite;
    bool isPlayerOn;

    // type 1 은 레버 당겨서 on/off 하는 버전 
    // type 2 는 loop 주기에 따라 켜졌다 꺼졌다 하는 버전 

    //type 1
    bool isElecAct; //현재 elec 이 작동하고 있는지의 여부 
    bool isCoroutineWork; //현재 코루틴이 진행중인지의 여부 
    bool isLeverActing; //현재 레버 코루틴이 작동중인지의 여부 
    public bool isElecActFirst; //true 이면 시작할 때 부터 켜져 있는 전기 

    //type 2
    public float elecTime; //전기가 흐르는 시간 
    public float elecDelay; //전기가 흐르는 시간 사이 텀 
    public float startOffset; //씬이 시작하고 몇 초 뒤부터 전기가 흐를 것인가 

    public Sprite[] elecSprite = new Sprite[5];
    //0 ~> 1 ~> 2 ~> 3 가 전기 생성
    //3 <~> 4 이 전기 흐르는 과정
    //3 ~> 2 ~> 1 ~> 0 이 전기 소멸 

    SpriteRenderer spr;
    SpriteRenderer leverSpr;
    BoxCollider2D coll;
    void Start()
    {
        spr = electric.GetComponent<SpriteRenderer>();
        leverSpr = GetComponent<SpriteRenderer>();
        coll = electric.GetComponent<BoxCollider2D>();

        leverSpr.sprite = leverSprite[0];

        if (elecType == 1 && isElecActFirst) //type1 이고 전기가 흐르는 게 디폴트면 전기 켜 놓음 
        {
            coll.enabled = true;
            spr.enabled = true;
            isElecAct = true;
            StartCoroutine("elecAni_work");
        }
        else
        {
            coll.enabled = false;
            spr.enabled = false;
        }
        
        if(elecType == 2) //type2 면 플레이어 상호작용 없으므로 그냥 작동 시작 
        {
            StartCoroutine(delayManager());
        }       
    }

    private void Update()
    {
        if (isPlayerOn && Input.GetKeyDown(KeyCode.E))
        {            
            if ((isCoroutineWork || isLeverActing) && elecType == 1) return;
            //type1: 전기 생성/소멸 애니메이션이 동작 중이면 작동x 

            if (elecType == 2) return; //elecType 이 2이면 버튼 없이 작동하는 elec 임 
            else
            {
                type1_elecAct();
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

    void type1_elecAct()
    {
        if (!isElecAct) //꺼져 있을땐 다시 켜야 함 
        {
            StartCoroutine(elecAni_appear());
            StartCoroutine(leverAct());
        }
        else
        {
            StartCoroutine(elecAni_dissapear());
            StartCoroutine(leverAct());
        }
    }

    IEnumerator leverAct()
    {
        isLeverActing = true;

        leverSpr.sprite = leverSprite[1];
        yield return new WaitForSeconds(0.5f);

        leverSpr.sprite = leverSprite[0];
        isLeverActing = false;
    }

    IEnumerator elecAni_appear() //전기가 발생하는 동작 
    {
        isElecAct = true;
        isCoroutineWork = true;

        spr.enabled = true;

        //전기 발생 
        spr.sprite = elecSprite[0];
        yield return new WaitForSeconds(0.1f);
        spr.sprite = elecSprite[1];
        yield return new WaitForSeconds(0.1f);

        coll.enabled = true; //중간쯤에 collider 활성화 

        spr.sprite = elecSprite[2];
        yield return new WaitForSeconds(0.1f);
        spr.sprite = elecSprite[3];
        yield return new WaitForSeconds(0.1f);

        isCoroutineWork = false;

        StartCoroutine("elecAni_work");
    }

    IEnumerator elecAni_work() //전기가 지지직거리는 동작 
    {
        while(true)
        {
            spr.sprite = elecSprite[4];
            yield return new WaitForSeconds(0.1f);
            spr.sprite = elecSprite[3];
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator elecAni_dissapear() //전기가 꺼지는 동작 
    {
        StopCoroutine("elecAni_work");

        isCoroutineWork = true;

        spr.sprite = elecSprite[4];
        yield return new WaitForSeconds(0.1f);
        spr.sprite = elecSprite[2];
        yield return new WaitForSeconds(0.1f);

        coll.enabled = false;
        //전기 사라짐 

        spr.sprite = elecSprite[1];
        yield return new WaitForSeconds(0.1f);
        spr.sprite = elecSprite[0];
        yield return new WaitForSeconds(0.1f);

        spr.enabled = false;

        isCoroutineWork = false;
        isElecAct = false;
    }

    IEnumerator delayManager() //type2 코드 
    {
        yield return new WaitForSeconds(startOffset);
        while (true)
        {
            StartCoroutine(shockActive());
            yield return new WaitForSeconds(elecDelay);
        }
    }

    IEnumerator shockActive() //type2 코드 
    {
        spr.enabled = true;

        //전기 발생 
        spr.sprite = elecSprite[0];
        yield return new WaitForSeconds(0.08f);
        spr.sprite = elecSprite[1];
        yield return new WaitForSeconds(0.08f);

        coll.enabled = true; //중간쯤에 collider 활성화 

        spr.sprite = elecSprite[2];
        yield return new WaitForSeconds(0.08f);
        spr.sprite = elecSprite[3];
        yield return new WaitForSeconds(0.08f);

        float frameTime = elecTime / 24;

        //지직거리기 
        for(int index=1; index <= 12; index++)
        {
            spr.sprite = elecSprite[4];
            yield return new WaitForSeconds(frameTime);
            spr.sprite = elecSprite[3];
            yield return new WaitForSeconds(frameTime);
        }
        coll.enabled = false;
        //전기 사라짐 
        spr.sprite = elecSprite[4];
        yield return new WaitForSeconds(0.08f);
        spr.sprite = elecSprite[2];
        yield return new WaitForSeconds(0.08f);
        spr.sprite = elecSprite[1];
        yield return new WaitForSeconds(0.08f);
        spr.sprite = elecSprite[0];
        yield return new WaitForSeconds(0.08f);

        spr.enabled = false;
    }
}
