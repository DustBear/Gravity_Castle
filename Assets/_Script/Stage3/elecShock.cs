using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elecShock : MonoBehaviour
{
    public GameObject electric; //������ ���� ������Ʈ 
    public int elecType;
    public Sprite[] leverSprite;
    bool isPlayerOn;

    // type 1 �� ���� ��ܼ� on/off �ϴ� ���� 
    // type 2 �� loop �ֱ⿡ ���� ������ ������ �ϴ� ���� 

    //type 1
    bool isElecAct; //���� elec �� �۵��ϰ� �ִ����� ���� 
    bool isCoroutineWork; //���� �ڷ�ƾ�� ������������ ���� 
    bool isLeverActing; //���� ���� �ڷ�ƾ�� �۵��������� ���� 
    public bool isElecActFirst; //true �̸� ������ �� ���� ���� �ִ� ���� 

    //type 2
    public float elecTime; //���Ⱑ �帣�� �ð� 
    public float elecDelay; //���Ⱑ �帣�� �ð� ���� �� 
    public float startOffset; //���� �����ϰ� �� �� �ں��� ���Ⱑ �带 ���ΰ� 

    public Sprite[] elecSprite = new Sprite[5];
    //0 ~> 1 ~> 2 ~> 3 �� ���� ����
    //3 <~> 4 �� ���� �帣�� ����
    //3 ~> 2 ~> 1 ~> 0 �� ���� �Ҹ� 

    SpriteRenderer spr;
    SpriteRenderer leverSpr;
    BoxCollider2D coll;
    void Start()
    {
        spr = electric.GetComponent<SpriteRenderer>();
        leverSpr = GetComponent<SpriteRenderer>();
        coll = electric.GetComponent<BoxCollider2D>();

        leverSpr.sprite = leverSprite[0];

        if (elecType == 1 && isElecActFirst) //type1 �̰� ���Ⱑ �帣�� �� ����Ʈ�� ���� �� ���� 
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
        
        if(elecType == 2) //type2 �� �÷��̾� ��ȣ�ۿ� �����Ƿ� �׳� �۵� ���� 
        {
            StartCoroutine(delayManager());
        }       
    }

    private void Update()
    {
        if (isPlayerOn && Input.GetKeyDown(KeyCode.E))
        {            
            if ((isCoroutineWork || isLeverActing) && elecType == 1) return;
            //type1: ���� ����/�Ҹ� �ִϸ��̼��� ���� ���̸� �۵�x 

            if (elecType == 2) return; //elecType �� 2�̸� ��ư ���� �۵��ϴ� elec �� 
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
        if (!isElecAct) //���� ������ �ٽ� �Ѿ� �� 
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

    IEnumerator elecAni_appear() //���Ⱑ �߻��ϴ� ���� 
    {
        isElecAct = true;
        isCoroutineWork = true;

        spr.enabled = true;

        //���� �߻� 
        spr.sprite = elecSprite[0];
        yield return new WaitForSeconds(0.1f);
        spr.sprite = elecSprite[1];
        yield return new WaitForSeconds(0.1f);

        coll.enabled = true; //�߰��뿡 collider Ȱ��ȭ 

        spr.sprite = elecSprite[2];
        yield return new WaitForSeconds(0.1f);
        spr.sprite = elecSprite[3];
        yield return new WaitForSeconds(0.1f);

        isCoroutineWork = false;

        StartCoroutine("elecAni_work");
    }

    IEnumerator elecAni_work() //���Ⱑ �������Ÿ��� ���� 
    {
        while(true)
        {
            spr.sprite = elecSprite[4];
            yield return new WaitForSeconds(0.1f);
            spr.sprite = elecSprite[3];
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator elecAni_dissapear() //���Ⱑ ������ ���� 
    {
        StopCoroutine("elecAni_work");

        isCoroutineWork = true;

        spr.sprite = elecSprite[4];
        yield return new WaitForSeconds(0.1f);
        spr.sprite = elecSprite[2];
        yield return new WaitForSeconds(0.1f);

        coll.enabled = false;
        //���� ����� 

        spr.sprite = elecSprite[1];
        yield return new WaitForSeconds(0.1f);
        spr.sprite = elecSprite[0];
        yield return new WaitForSeconds(0.1f);

        spr.enabled = false;

        isCoroutineWork = false;
        isElecAct = false;
    }

    IEnumerator delayManager() //type2 �ڵ� 
    {
        yield return new WaitForSeconds(startOffset);
        while (true)
        {
            StartCoroutine(shockActive());
            yield return new WaitForSeconds(elecDelay);
        }
    }

    IEnumerator shockActive() //type2 �ڵ� 
    {
        spr.enabled = true;

        //���� �߻� 
        spr.sprite = elecSprite[0];
        yield return new WaitForSeconds(0.08f);
        spr.sprite = elecSprite[1];
        yield return new WaitForSeconds(0.08f);

        coll.enabled = true; //�߰��뿡 collider Ȱ��ȭ 

        spr.sprite = elecSprite[2];
        yield return new WaitForSeconds(0.08f);
        spr.sprite = elecSprite[3];
        yield return new WaitForSeconds(0.08f);

        float frameTime = elecTime / 24;

        //�����Ÿ��� 
        for(int index=1; index <= 12; index++)
        {
            spr.sprite = elecSprite[4];
            yield return new WaitForSeconds(frameTime);
            spr.sprite = elecSprite[3];
            yield return new WaitForSeconds(frameTime);
        }
        coll.enabled = false;
        //���� ����� 
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
