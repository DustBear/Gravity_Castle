using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class advancedStageDoor : MonoBehaviour
{
    public Sprite[] spritesGroup = new Sprite[4]; //0�� ���ð� ���� ����, 3�� ���ð� ������ Ȱ��ȭ�� ����
    public BoxCollider2D spikeColl;
    Rigidbody2D rigid;
    public int doorType;
    //type1 �� ������ ������ ������ ��ư ������ �ö󰡴� Ÿ��(sprite0�� ����, sprite3�� ��)
    //type2 �� ���� �ö� �ְ� �ݶ��̴� ����ϸ� �������� Ÿ��(sprite3�� ����, sprite0�� ��)

    public float doorLength; //stageDoor�� ����(3,4,5): ���� �� ���ڸ�ŭ ���� �ö󰡾� ��
    public float doorPeriod; //���� ������ �ö󰡰�/������ �� �ɸ��� �ð�
    public GameObject thresholdPoint; //���� Ȱ��ȭ�� ������ �� savePoint/key �� ȹ�������� ���� ���� ���·� ����
    public int doorActiveThreshold; //thresholdPoint �� �ٸ� ���� �־ ������Ʈ�� ������ �� ���� ��� �������� �Է� 
    
    SpriteRenderer spr;
    AudioSource[] sounds;

    AudioSource playOnceSound;
    AudioSource loopSound;

    public AudioClip doorShakeSound;
    public AudioClip doorOpenLoopSound;
    public AudioClip doorHitSound;

    Vector3 initialPos; //�� ó�� �ʱ�ȭ �� ��ġ

    public Vector2[] spikeOffsetGroup; 
    //[0]�� ���� �� ����, [3]�� ���� Ƣ��� ���� 

    public bool disposable; 
    //�� ������ ������ ���� ���̵� �������� ���� �����Ϳ� ���Ե� ������ �������� �� ���� '�Ź�' �� ���·� �ʱ�ȭ�ȴ�

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        sounds = GetComponents<AudioSource>();

        playOnceSound = sounds[0];
        loopSound = sounds[1];

        playOnceSound.loop = false;
        loopSound.loop = true;

        rigid = GetComponent<Rigidbody2D>();
        if(thresholdPoint.gameObject != null)//disposable �� ������ ���� ���� thresholdPoint �� ���� 
        {
            doorActiveThreshold = thresholdPoint.GetComponent<SavePoint>().achievementNum;
        }      
    }
    void Start() //door �� localPos �� �������� ������: ȸ���� stageDoor �׷� ��ü�� ��� �� 
    {
        initialPos = transform.position; //�ʱ� ��ġ ���� 

        // ���� ��������Ʈ �� ������ ���� 
        if (disposable) //��ȸ�� ���� ��� 
        {
            if(doorType == 1) //������ ���°� ����Ʈ 
            {
                spr.sprite = spritesGroup[3];
                spikeColl.offset = spikeOffsetGroup[3];
            }

            else if(doorType == 2) //�ö� ���°� ����Ʈ 
            {
                spr.sprite = spritesGroup[0];
                spikeColl.offset = spikeOffsetGroup[0];
            }
        }       
        else //�Ϲ����� ���� �������� ���� �ִ� ��
        {
            if (GameManager.instance.gameData.curAchievementNum >= doorActiveThreshold) //���� �̹� ���� ���̸� 
            {
                if (doorType == 1) //��ư ������ �ö󰡴� �� ~> �ö� ä�� �־�� ��
                {
                    spr.sprite = spritesGroup[0];
                    transform.position = initialPos + transform.up * doorLength;
                    spikeColl.offset = spikeOffsetGroup[0];
                }
                else //�ݶ��̴� �����ϸ� �������� �� ~> ������ ä�� �־�� ��
                {
                    spr.sprite = spritesGroup[3];
                    transform.position = initialPos - transform.up * doorLength;
                    spikeColl.offset = spikeOffsetGroup[3];
                }
            }

            else
            {
                if (doorType == 1)
                {
                    spr.sprite = spritesGroup[3];
                    spikeColl.offset = spikeOffsetGroup[3];
                }
                else
                {
                    spr.sprite = spritesGroup[0];
                    spikeColl.offset = spikeOffsetGroup[0];
                }
            }
        }       
    }

    void Update()
    {
        
    }

    public void doorMove()
    {
        if(doorType == 1)
        {
            StartCoroutine("doorUp");
        }
        else if(doorType == 2)
        {
            StartCoroutine("doorDown");
        }
    }

    IEnumerator doorUp() 
    {
        //���� ���� ���¿���
        //(1) ���� ��Ȱ��ȭ 
        //(2) �� ����
        //(3) ���� �ö� 

        StartCoroutine("spikeDeActive");
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("doorShake");
        yield return new WaitForSeconds(0.6f);

        loopSound.clip = doorOpenLoopSound;
        loopSound.Play();

        float moveSpeed = doorLength / doorPeriod; //���� �ö󰡴� �ӵ� 

        rigid.velocity = transform.up * moveSpeed;

        yield return new WaitForSeconds(doorPeriod - 0.1f);
        playOnceSound.Stop();
        playOnceSound.clip = doorHitSound;
        playOnceSound.Play();

        yield return new WaitForSeconds(0.1f);
        rigid.velocity = Vector3.zero;
        transform.position = initialPos + transform.up * doorLength; //���� ��ġ �ö�ä�� ���� 

        loopSound.Stop();
    }

    IEnumerator doorDown()
    {
        //���� ���� ���¿���
        //(1) �� ���� 
        //(2) ���� ������
        //(3) ���� Ȱ��ȭ 

        StartCoroutine("doorShake");
        yield return new WaitForSeconds(0.6f);

        loopSound.clip = doorOpenLoopSound;
        loopSound.Play();

        float moveSpeed = doorLength / doorPeriod; //���� �ö󰡴� �ӵ� 

        rigid.velocity = -transform.up * moveSpeed;

        yield return new WaitForSeconds(doorPeriod-0.1f);
        playOnceSound.Stop();
        playOnceSound.clip = doorHitSound;
        playOnceSound.Play();

        yield return new WaitForSeconds(0.1f);

        rigid.velocity = Vector3.zero;
        transform.position = initialPos - transform.up * doorLength; //���� ��ġ ������ä�� ���� 

        loopSound.Stop();

        yield return new WaitForSeconds(0.5f);
        StartCoroutine("spikeActive");
    }

    IEnumerator spikeActive() //���� Ȱ��ȭ�ϴ� �Լ� 
    {
        for(int index=0; index<4; index++)
        {
            spr.sprite = spritesGroup[index];
            spikeColl.offset = spikeOffsetGroup[index];
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator spikeDeActive() //���� ��Ȱ��ȭ��Ű�� �Լ� 
    {
        for (int index = 3; index >=0; index--)
        {
            spr.sprite = spritesGroup[index];
            spikeColl.offset = spikeOffsetGroup[index];
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator doorShake() //���� ���� ��, ���� ���� �� ��鸲 ~> 0.48�ʰ� ��鸲 
    {
        playOnceSound.Stop();
        playOnceSound.clip = doorShakeSound;
        playOnceSound.Play();

        float shakeDegree = 0.03f; //���Ʒ��� ��鸲
        for(int index=0; index<3; index++)
        {
            transform.position = initialPos + transform.up * shakeDegree;
            yield return new WaitForSeconds(0.04f);

            transform.position = initialPos - transform.up * shakeDegree;
            yield return new WaitForSeconds(0.04f);

            transform.position = initialPos - transform.up * shakeDegree;
            yield return new WaitForSeconds(0.04f);

            transform.position = initialPos + transform.up * shakeDegree;
            yield return new WaitForSeconds(0.04f);

        }
    }
}
