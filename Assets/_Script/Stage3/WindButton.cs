using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindButton : MonoBehaviour
{
    [SerializeField] WindHome windHome;
    [SerializeField] GameObject windZone;
    [SerializeField] GameObject wind;

    public Sprite[] leverSprite;
    //[0]�� ����, [1]�� ���������� ������ ���� 
  
    public int windType;
    // 1�̸� ������ on/off �ϴ� ȯǳ��
    // on ���¸� ������ sprite[0], off ���¸� ������ sprite[1] �� ��/�츦 �Դٰ����ؾ� �� 

    // 2�̸� Ÿ�̸� ���� ȯǳ��/ ��ҿ� �����ִٰ� ��ư������ ������ ��� or ��ҿ� �����ִٰ� ��ư������ ������ ���
    // on ���¸� ������ sprite[0], off ���¸� ������ sprite[1], Ÿ�̸Ӱ� �� �Ǹ� ���� ��������Ʈ�� �ڵ����� �ٲ��� �� 

    // 3�̸� Ư�� �ֱ⿡ ���� ������ �����ٸ� �ݺ��ϴ� ȯǳ�� 
    // ������ spr �� ���� ������ �ʰ� ���ܵ־� �� 

    //type1
    bool isActive; //���� windHome �� �۵��ϰ� �ִ����� ����     

    //type 2
    public bool isTimerAct; //���� Ÿ�̸Ӱ� �귯���� �ִ����� ���� 
    public float TimerActiveTime; //Ÿ�̸� ����� button�� �۵��ϴ� �ð�

    //type1 && type2 
    public bool isActDefault; //true �̸� �����ִ°� ����Ʈ�� ȯǳ�� 

    //type 3 : �� ���� ��ư�� �ʿ���� ȯǳ�Ⱑ �˾Ƽ� �۵��ϴ� ����̹Ƿ�, ��ư�� ������ �ʴ� ���� ���ܵΰ� ����ȭ���Ѿ� �� 
    public float loopOffset; //�� ó�� �����ϰ� ȯǳ�Ⱑ ����������� �ð� 
    public float loopDelay; //ȯǳ�Ⱑ ������ �ٽ� ����������� �ð�
    public float loopActive; //ȯǳ�Ⱑ ���� �ִ� �ð� 

    BoxCollider2D windZoneColl;
    SpriteRenderer spr;

    bool isPlayerOn= false;

    AudioSource sound;
    void Awake()
    {
        windZoneColl = windZone.GetComponent<BoxCollider2D>();
        spr = GetComponent<SpriteRenderer>();
        sound = GetComponent<AudioSource>();

        //ȯǳ���� �ʱ���� ���� 

        if(isActDefault && (windType == 1 || windType == 2)) 
            //ȯǳ�Ⱑ type1 or type2 �̸鼭 ���� ���°� ����Ʈ�̸� ȯǳ��� ó������ ������ �� 
        {
            windHome.enabled = true;
            windZoneColl.enabled = true;
            wind.SetActive(true);

            isActive = true;
            spr.sprite = leverSprite[1]; 
        }
        else
            //�� ���� ���� �� �� 
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
        if(windType == 3) //type3�̸� �÷��̾ ��ȣ�ۿ� �� �ʿ� ���� �ٷ� �ڷ�ƾ ������ 
        {
            StartCoroutine(type3_windAct());
        }
    }
    private void Update()
    {
        if (windType == 2 && isTimerAct) return;
        //type2 ȯǳ���� ��� Ÿ�̸� �۵����̸� �÷��̾� ���� ���� 

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

    void type1_windAct() //�۵��ϰ� �ִ� ȯǳ��� ���� ���� �ִ� ȯǳ��� Ű�� 
    {
        if (!isActive) //���� ������ �ٽ� �Ѿ� �� 
        {
            windHome.enabled = true;
            windZoneColl.enabled = true;
            wind.SetActive(true);

            isActive = true;
            spr.sprite = leverSprite[1]; //���� Ŵ 
        }
        else
        {
            windHome.enabled = false;
            windZoneColl.enabled = false;
            wind.SetActive(false);

            isActive = false;
            spr.sprite = leverSprite[0]; //���� �� 
        }        
    }

    IEnumerator type2_windAct() //���� ���� Ÿ�̸Ӱ� ����, Ÿ�̸� ������ �ٽ� ������� ���ƿ� 
    {
        if (isActDefault) //�����ִ� ���°� ����Ʈ�̸� 
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
}