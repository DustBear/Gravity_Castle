using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class advancedStageDoor : MonoBehaviour
{
    public Sprite[] spritesGroup = new Sprite[4]; //0�� ���ð� ���� ����, 3�� ���ð� ������ Ȱ��ȭ�� ����
    public BoxCollider2D spikeColl;
    public int doorType;
    //type1 �� ������ ������ ������ ��ư ������ �ö󰡴� Ÿ��(sprite0�� ����, sprite3�� ��)
    //type2 �� ���� �ö� �ְ� �ݶ��̴� ����ϸ� �������� Ÿ��(sprite3�� ����, sprite0�� ��)

    public float doorLength; //stageDoor�� ����(3,4,5): ���� �� ���ڸ�ŭ ���� �ö󰡾� ��
    public float doorPeriod; //���� ������ �ö󰡰�/������ �� �ɸ��� �ð�
    public GameObject thresholdPoint; //���� Ȱ��ȭ�� ������ �� savePoint/key �� ȹ�������� ���� ���� ���·� ����
    public int doorActiveThreshold; //thresholdPoint �� �ٸ� ���� �־ ������Ʈ�� ������ �� ���� ��� �������� �Է� 
    
    SpriteRenderer spr;
    AudioSource sound;

    Vector2 initialPos; //�� ó�� �ʱ�ȭ �� ��ġ

    public Vector2[] spikeOffsetGroup; 
    //[0]�� ���� �� ����, [3]�� ���� Ƣ��� ���� 

    public bool disposable; 
    //�� ������ ������ ���� ���̵� �������� ���� �����Ϳ� ���Ե� ������ �������� �� ���� '�Ź�' �� ���·� �ʱ�ȭ�ȴ�

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        sound = GetComponent<AudioSource>();
        if(thresholdPoint.gameObject != null)//disposable �� ������ ���� ���� thresholdPoint �� ���� 
        {
            doorActiveThreshold = thresholdPoint.GetComponent<SavePoint>().achievementNum;
        }      
    }
    void Start() //door �� localPos �� �������� ������: ȸ���� stageDoor �׷� ��ü�� ��� �� 
    {
        initialPos = transform.localPosition;

        if (disposable)
        {
            if(doorType == 1)
            {
                spr.sprite = spritesGroup[3];
                spikeColl.offset = spikeOffsetGroup[3];
            }

            else if(doorType == 2)
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
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + doorLength, 0);
                    spikeColl.offset = spikeOffsetGroup[0];
                }
                else //�ݶ��̴� �����ϸ� �������� �� ~> ������ ä�� �־�� ��
                {
                    spr.sprite = spritesGroup[3];
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - doorLength, 0);
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

        if(sound != null)
        {
            sound.Play();
        }
       
        StartCoroutine("spikeDeActive");
        yield return new WaitForSeconds(1.5f);
        StartCoroutine("doorShake");
        yield return new WaitForSeconds(0.6f);

        int frameIndex = 1;
        while(frameIndex <= 50)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + doorLength/50, 0);

            frameIndex++;
            yield return new WaitForSeconds(doorPeriod / 50);
        }
        if (sound != null)
        {
            sound.Stop();
        }
    }

    IEnumerator doorDown()
    {
        //���� ���� ���¿���
        //(1) �� ���� 
        //(2) ���� ������
        //(3) ���� Ȱ��ȭ 
        if (sound != null) sound.Play();

        StartCoroutine("doorShake");
        yield return new WaitForSeconds(0.6f);

        int frameIndex = 1;
        while (frameIndex <= 50)
        {
            float doorYPos = initialPos.y - (doorLength / 50 * frameIndex);
            transform.localPosition = new Vector3(transform.localPosition.x, doorYPos, 0);

            frameIndex++;
            yield return new WaitForSeconds(doorPeriod / 50);
        }

        StartCoroutine("spikeActive");
        if (sound != null) sound.Stop();
    }

    IEnumerator spikeActive() //���� Ȱ��ȭ�ϴ� �Լ� 
    {
        for(int index=0; index<4; index++)
        {
            spr.sprite = spritesGroup[index];
            spikeColl.offset = spikeOffsetGroup[index];
            yield return new WaitForSeconds(0.25f);
        }
    }

    IEnumerator spikeDeActive() //���� ��Ȱ��ȭ��Ű�� �Լ� 
    {
        for (int index = 3; index >=0; index--)
        {
            spr.sprite = spritesGroup[index];
            spikeColl.offset = spikeOffsetGroup[index];
            yield return new WaitForSeconds(0.25f);
        }
    }

    IEnumerator doorShake() //���� ���� ��, ���� ���� �� ��鸲 ~> 0.48�ʰ� ��鸲 
    {
        float shakeDegree = 0.03f; //���Ʒ��� ��鸲
        for(int index=0; index<3; index++)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, initialPos.y + shakeDegree, 0);
            yield return new WaitForSeconds(0.04f);
            transform.localPosition = new Vector3(transform.localPosition.x, initialPos.y - shakeDegree, 0);
            yield return new WaitForSeconds(0.04f);
            transform.localPosition = new Vector3(transform.localPosition.x, initialPos.y - shakeDegree, 0);
            yield return new WaitForSeconds(0.04f);
            transform.localPosition = new Vector3(transform.localPosition.x, initialPos.y + shakeDegree, 0);
            yield return new WaitForSeconds(0.04f);
        }
    }
}
