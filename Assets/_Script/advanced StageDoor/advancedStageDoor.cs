using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class advancedStageDoor : MonoBehaviour
{
    public Sprite[] spritesGroup = new Sprite[4]; //0�� ���ð� ���� ����, 3�� ���ð� ������ Ȱ��ȭ�� ����
    public GameObject spikeColl;
    public int doorType;
    //type1 �� ������ ������ ������ ��ư ������ �ö󰡴� Ÿ��(sprite0�� ����, sprite3�� ��)
    //type2 �� ���� �ö� �ְ� �ݶ��̴� ����ϸ� �������� Ÿ��(sprite3�� ����, sprite0�� ��)

    public float doorLength; //stageDoor�� ����(3,4,5): ���� �� ���ڸ�ŭ ���� �ö󰡾� ��
    public float doorPeriod; //���� ������ �ö󰡰�/������ �� �ɸ��� �ð�
    public int doorActiveThreshold; //�÷��̾��� achieveNum �� �� ���� '�ʰ�' �̸� ��Ȱ��ȭ�Ѵ�.    

    SpriteRenderer spr;
    AudioSource sound;

    Vector2 initialPos; //�� ó�� �ʱ�ȭ �� ��ġ

    public bool isOnSideStage;
    [SerializeField] int doorNum; //1���� ����
    //�� stageDoor�� side stage���� �ִ����� ���� üũ ~> �ش� ��ȣ�� sideStage�� Ȱ��ȭ�Ǿ� ���� ������ ���� ���·� ����

    public bool disposable; 
    //�� ������ ������ ���� ���̵� �������� ���� �����Ϳ� ���Ե� ������ �������� �� ���� '�Ź�' �� ���·� �ʱ�ȭ�ȴ�

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        sound = GetComponent<AudioSource>();
    }
    void Start()
    {
        initialPos = transform.localPosition;

        if (disposable)
        {
            if(doorType == 1)
            {
                spr.sprite = spritesGroup[3];
                spikeColl.SetActive(true);
            }

            else if(doorType == 2)
            {
                spr.sprite = spritesGroup[0];
                spikeColl.SetActive(false);
            }
        }

        else if (isOnSideStage) //���̵彺������ unlock ���� �����ϴ� ��
        {
            if (GameManager.instance.gameData.sideStageUnlock[doorNum - 1]) //�ش� ���̵彺�������� �̹� unlock �� ���¸�
            {
                spr.sprite = spritesGroup[0]; //���� �̹� �ö� �־�� ��
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + doorLength, 0);
                spikeColl.SetActive(false);
            }
            else
            {
                //�ش� ���������� ���� unlock���� �������¸� ���� ������ ���·� �־�� ��
                spr.sprite = spritesGroup[3];
                spikeColl.SetActive(true);
            }
        }
        else //�Ϲ����� ���� �������� ���� �ִ� ��
        {
            if (GameManager.instance.gameData.curAchievementNum > doorActiveThreshold) //���� �̹� ���� ���̸� 
            {
                if (doorType == 1) //��ư ������ �ö󰡴� �� ~> �ö� ä�� �־�� ��
                {
                    spr.sprite = spritesGroup[0];
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + doorLength, 0);
                    spikeColl.SetActive(false);
                }
                else //�ݶ��̴� �����ϸ� �������� �� ~> ������ ä�� �־�� ��
                {
                    spr.sprite = spritesGroup[3];
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - doorLength, 0);
                    spikeColl.SetActive(true);
                }
            }

            else
            {
                if (doorType == 1)
                {
                    spr.sprite = spritesGroup[3];
                    spikeColl.SetActive(true);
                }
                else
                {
                    spr.sprite = spritesGroup[0];
                    spikeColl.SetActive(false);
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
            yield return new WaitForSeconds(0.5f);
        }

        spikeColl.SetActive(true);
    }

    IEnumerator spikeDeActive() //���� ��Ȱ��ȭ��Ű�� �Լ� 
    {
        for (int index = 3; index >=0; index--)
        {
            spr.sprite = spritesGroup[index];
            yield return new WaitForSeconds(0.5f);
        }

        spikeColl.SetActive(false);
    }

    IEnumerator doorShake() //���� ���� ��, ���� ���� �� ��鸲 ~> 0.6�ʰ� ��鸲 
    {
        float shakeDegree = 0.03f; //���Ʒ��� ��鸲
        for(int index=0; index<3; index++)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, initialPos.y + shakeDegree, 0);
            yield return new WaitForSeconds(0.05f);
            transform.localPosition = new Vector3(transform.localPosition.x, initialPos.y - shakeDegree, 0);
            yield return new WaitForSeconds(0.05f);
            transform.localPosition = new Vector3(transform.localPosition.x, initialPos.y - shakeDegree, 0);
            yield return new WaitForSeconds(0.05f);
            transform.localPosition = new Vector3(transform.localPosition.x, initialPos.y + shakeDegree, 0);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
