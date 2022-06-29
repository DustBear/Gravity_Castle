using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class advancedStageDoor : MonoBehaviour
{
    public Sprite[] spritesGroup = new Sprite[4]; //0은 가시가 없는 버전, 3는 가시가 완전히 활성화된 버전
    public GameObject spikeColl;
    public int doorType;
    //type1 은 원래는 내려가 있지만 버튼 누르면 올라가는 타입(sprite0로 시작, sprite3로 끝)
    //type2 는 원래 올라가 있고 콜라이더 통과하면 내려가는 타입(sprite3로 시작, sprite0로 끝)

    public float doorLength; //stageDoor의 길이(3,4,5): 문은 이 숫자만큼 위로 올라가야 함
    public float doorPeriod; //문이 완전히 올라가고/닫히는 데 걸리는 시간
    public int doorActiveThreshold; //플레이어의 achieveNum 이 이 숫자 '초과' 이면 비활성화한다.    

    SpriteRenderer spr;
    AudioSource sound;

    Vector2 initialPos; //맨 처음 초기화 할 위치

    public bool isOnSideStage;
    [SerializeField] int doorNum; //1부터 시작
    //이 stageDoor가 side stage내에 있는지의 여부 체크 ~> 해당 번호의 sideStage가 활성화되어 있을 때에만 열린 상태로 존재

    public bool disposable; 
    //이 조건이 설정된 문은 사이드 스테이지 내의 퍼즐기믹에 포함된 문으로 리스폰할 때 마다 '매번' 원 상태로 초기화된다

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

        else if (isOnSideStage) //사이드스테이지 unlock 여부 결정하는 문
        {
            if (GameManager.instance.gameData.sideStageUnlock[doorNum - 1]) //해당 사이드스테이지가 이미 unlock 된 상태면
            {
                spr.sprite = spritesGroup[0]; //문은 이미 올라가 있어야 함
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + doorLength, 0);
                spikeColl.SetActive(false);
            }
            else
            {
                //해당 스테이지가 아직 unlock되지 않은상태면 문은 내려온 상태로 있어야 함
                spr.sprite = spritesGroup[3];
                spikeColl.SetActive(true);
            }
        }
        else //일반적인 메인 스테이지 내에 있는 문
        {
            if (GameManager.instance.gameData.curAchievementNum > doorActiveThreshold) //만약 이미 열린 문이면 
            {
                if (doorType == 1) //버튼 누르면 올라가는 문 ~> 올라간 채로 있어야 함
                {
                    spr.sprite = spritesGroup[0];
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + doorLength, 0);
                    spikeColl.SetActive(false);
                }
                else //콜라이더 반응하면 내려오는 문 ~> 내려온 채로 있어야 함
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
        //문이 닫힌 상태에서
        //(1) 가시 비활성화 
        //(2) 문 진동
        //(3) 문이 올라감 

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
        //문이 열린 상태에서
        //(1) 문 진동 
        //(2) 문이 내려감
        //(3) 가시 활성화 
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

    IEnumerator spikeActive() //가시 활성화하는 함수 
    {
        for(int index=0; index<4; index++)
        {
            spr.sprite = spritesGroup[index];
            yield return new WaitForSeconds(0.5f);
        }

        spikeColl.SetActive(true);
    }

    IEnumerator spikeDeActive() //가시 비활성화시키는 함수 
    {
        for (int index = 3; index >=0; index--)
        {
            spr.sprite = spritesGroup[index];
            yield return new WaitForSeconds(0.5f);
        }

        spikeColl.SetActive(false);
    }

    IEnumerator doorShake() //문이 열릴 때, 문이 닫힐 때 흔들림 ~> 0.6초간 흔들림 
    {
        float shakeDegree = 0.03f; //위아래로 흔들림
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
