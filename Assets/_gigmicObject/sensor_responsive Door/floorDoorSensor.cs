using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floorDoorSensor : MonoBehaviour
{
    //���� ���� �÷��̾� or Box�� �ö󰡸� ������ �ν��ؼ� ���� ������

    [SerializeField] bool isPlayerOn;
    [SerializeField] bool isObjectOn;

    public GameObject doorCol;
    SpriteRenderer spr;
    SpriteRenderer sensorSpr;
    BoxCollider2D doorBox;

    public Sprite[] doorSpr_idle;
    public Sprite[] doorSpr_active;

    public Sprite[] sensorLight; //[0]�� �� ���� ���� [2]�� �� ���� ����

    IEnumerator curCoroutine; //�� ���� �ϳ��� �ڷ�ƾ�� �۵��ϵ��� �� 

    bool isDoorActing = false;

    public AudioSource sound;
    public AudioSource sound_loop;

    public AudioClip sensorOn;
    public AudioClip sensorOff;
    public AudioClip sensorIdle;

    private void Awake()
    {
        spr = doorCol.GetComponent<SpriteRenderer>();
        sensorSpr = GetComponent<SpriteRenderer>();
        doorBox = doorCol.GetComponent<BoxCollider2D>();
    }

    void Start()
    { 
        spr.sprite = doorSpr_idle[0];
        sensorSpr.sprite = sensorLight[0];

        if (curCoroutine != null)
        {
            StopCoroutine(curCoroutine);
        }

        curCoroutine = doorIdle();
        StartCoroutine(curCoroutine); //�����ϸ� �⺻ �ִϸ��̼� ����
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
                sound.PlayOneShot(sensorOn);
                sound_loop.Play();

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
                sound.PlayOneShot(sensorOn);
                sound_loop.Play();

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
            if (!isObjectOn) //���� ���� �ƹ��͵� ���� ��� ~> �� ���� 
            {
                sound.PlayOneShot(sensorOff);
                sound_loop.Stop();

                StopCoroutine(curCoroutine);
                curCoroutine = doorClose();

                StartCoroutine(curCoroutine);
            }
        }
        else if (collision.tag == "Platform")
        {
            isObjectOn = false;
            if (!isPlayerOn) //���� ���� �ƹ��͵� ���� ��� ~> �� ���� 
            {
                sound.PlayOneShot(sensorOff);
                sound_loop.Stop();

                StopCoroutine(curCoroutine);
                curCoroutine = doorClose();

                StartCoroutine(curCoroutine);
            }
        }
    }
   
    IEnumerator doorIdle() //������ ���� �� �۵��ϴ� �ִϸ��̼�
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
        for(int index=0; index<=2; index++)
        {
            sensorSpr.sprite = sensorLight[index];
        }

        for (int index = 0; index < doorSpr_active.Length; index++)
        {
            spr.sprite = doorSpr_active[index];
            yield return new WaitForSeconds(0.05f);
        }

        spr.enabled = false; //�� ���� �ִϸ��̼� ������ �ƿ� �� ���̰� �� 
        doorBox.enabled = false;

        isDoorActing = false;
    }

    IEnumerator doorClose()
    {
        isDoorActing = true;
        for (int index = 2; index >= 0; index--)
        {
            sensorSpr.sprite = sensorLight[index];
        }

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
