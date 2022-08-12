using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elecShock : MonoBehaviour
{
    public GameObject electric; //통제할 전기 오브젝트 
    public int elecType;
    // type 1 은 버튼 눌러서 on/off 하는 버전 
    // type 2 는 loop 주기에 따라 켜졌다 꺼졌다 하는 버전 

    //type 1
    bool isElecAct; //현재 elec 이 작동하고 있는지의 여부 
    bool isButtonClicked; //현재 버튼이 눌려져 있는지의 여부 
    bool isCoroutineWork; //현재 코루틴이 진행중인지의 여부 
    public float buttonClickDelay; //버튼이 눌려져 있는 시간 
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
    BoxCollider2D coll;
    void Start()
    {
        spr = electric.GetComponent<SpriteRenderer>();
        coll = electric.GetComponent<BoxCollider2D>();

        if(elecType == 1 && isElecActFirst)
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
        
        if(elecType == 2)
        {
            StartCoroutine(delayManager());
        }       
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((isButtonClicked || isCoroutineWork) && elecType == 1) return;
        //type1: 버튼이 다시 올라오기 전이거나 전기 생성/소멸 애니메이션이 동작 중이면 작동x 
        if (elecType == 2) return; //elecType 이 2이면 버튼 없이 작동하는 elec 임 
        
        if (collision.gameObject.tag == "Player" && collision.transform.up == transform.up)
        {
            if (elecType == 1)
            {
                type1_elecAct();
            }           
        }
    }

    void type1_elecAct()
    {
        if (!isElecAct) //꺼져 있을땐 다시 켜야 함 
        {
            StartCoroutine(elecAni_appear());
            buttonClick(); //버튼이 눌림 
            Invoke("buttonReturn", buttonClickDelay);
        }
        else
        {
            StartCoroutine(elecAni_dissapear());
            Debug.Log("elec stop");

            buttonClick(); //버튼이 눌림 
            Invoke("buttonReturn", buttonClickDelay);
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

    IEnumerator shockActive()
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
