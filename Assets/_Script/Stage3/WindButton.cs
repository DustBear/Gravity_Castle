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
    // 1�̸� ��ư���� on/off �ϴ� ȯǳ��
    // 2�̸� Ÿ�̸� ���� ȯǳ��/ ��ҿ� �����ִٰ� ��ư������ ������ ��� or ��ҿ� �����ִٰ� ��ư������ ������ ���
    // 3�̸� Ư�� �ֱ⿡ ���� ������ �����ٸ� �ݺ��ϴ� ȯǳ�� 

    //type1
    bool isActive; //���� windHome �� �۵��ϰ� �ִ����� ���� 
    bool isButtonClicked; //���� ��ư�� ������ �ִ����� ���� 
    public float buttonClickDelay; //��ư�� ������ �ִ� �ð� 

    //type 2
    public bool isActDefault; //true �̸� �����ִ°� ����Ʈ�� ȯǳ�� 
    public bool isTimerAct; //���� Ÿ�̸Ӱ� �귯���� �ִ����� ���� 
    public float TimerActiveTime; //Ÿ�̸� ����� button�� �۵��ϴ� �ð�

    //type 3 : �� ���� ��ư�� �ʿ���� ȯǳ�Ⱑ �˾Ƽ� �۵��ϴ� ����̹Ƿ�, ��ư�� ������ �ʴ� ���� ���ܵΰ� ����ȭ���Ѿ� �� 
    public float loopOffset; //�� ó�� �����ϰ� ȯǳ�Ⱑ ����������� �ð� 
    public float loopDelay; //ȯǳ�Ⱑ ������ �ٽ� ����������� �ð�
    public float loopActive; //ȯǳ�Ⱑ ���� �ִ� �ð� 

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
        else if (windType == 2 && isActDefault) //type 2 �ε� ���� ���°� default �� ȯǳ���� ���
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
        //type1: ��ư�� �ٽ� �ö���� ���̸� �۵�x
        if (isTimerAct && windType == 2) return;
        //type2: Ÿ�̸� �ð��� �귯���� �߿��� ��ư ������ ����x 

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

    void type1_windAct() //�۵��ϰ� �ִ� ȯǳ��� ���� ���� �ִ� ȯǳ��� Ű�� 
    {
        if (!isActive) //���� ������ �ٽ� �Ѿ� �� 
        {
            windHome.enabled = true;
            windZoneColl.enabled = true;
            wind.SetActive(true);

            isActive = true;
            buttonClick(); //��ư�� ���� 
            Invoke("buttonReturn", buttonClickDelay);
        }
        else
        {
            windHome.enabled = false;
            windZoneColl.enabled = false;
            wind.SetActive(false);

            isActive = false;
            buttonClick(); //��ư�� ���� 
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
            buttonClick(); //��ư�� ���� 

            Invoke("type2_windOn", TimerActiveTime); //������ �ð��� �帥 �� ȯǳ�� ������ �� 
            Invoke("buttonReturn", buttonClickDelay);
        }
        else
        {
            windHome.enabled = true;
            windZoneColl.enabled = true;
            wind.SetActive(true);

            isTimerAct = true;
            buttonClick(); //��ư�� ���� 

            Invoke("type2_windOff", TimerActiveTime); //������ �ð��� �帥 �� ȯǳ�� ������ �� 
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
            //ȯǳ�� �ѱ� 
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