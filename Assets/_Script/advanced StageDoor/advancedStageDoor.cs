using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class advancedStageDoor : MonoBehaviour
{
    public Sprite[] spritesGroup = new Sprite[4]; //0은 가시가 없는 버전, 3는 가시가 완전히 활성화된 버전
    public BoxCollider2D spikeColl;
    Rigidbody2D rigid;
    public int doorType;
    //type1 은 원래는 내려가 있지만 버튼 누르면 올라가는 타입(sprite0로 시작, sprite3로 끝)
    //type2 는 원래 올라가 있고 콜라이더 통과하면 내려가는 타입(sprite3로 시작, sprite0로 끝)

    public float doorLength; //stageDoor의 길이(3,4,5): 문은 이 숫자만큼 위로 올라가야 함
    public float doorPeriod; //문이 완전히 올라가고/닫히는 데 걸리는 시간
    public GameObject thresholdPoint; //씬을 활성화한 시점에 이 savePoint/key 를 획득했으면 문이 열린 상태로 유지
    public int doorActiveThreshold; //thresholdPoint 가 다른 씬에 있어서 오브젝트를 가져올 수 없는 경우 수동으로 입력 
    
    SpriteRenderer spr;
    AudioSource[] sounds;

    AudioSource playOnceSound;
    AudioSource loopSound;

    public AudioClip doorShakeSound;
    public AudioClip doorOpenLoopSound;
    public AudioClip doorHitSound;

    Vector3 initialPos; //맨 처음 초기화 할 위치

    public Vector2[] spikeOffsetGroup; 
    //[0]이 가시 들어간 상태, [3]이 가시 튀어나온 상태 

    public bool disposable; 
    //이 조건이 설정된 문은 사이드 스테이지 내의 퍼즐기믹에 포함된 문으로 리스폰할 때 마다 '매번' 원 상태로 초기화된다

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        sounds = GetComponents<AudioSource>();

        playOnceSound = sounds[0];
        loopSound = sounds[1];

        playOnceSound.loop = false;
        loopSound.loop = true;

        rigid = GetComponent<Rigidbody2D>();
        if(thresholdPoint.gameObject != null)//disposable 로 설정된 문은 따로 thresholdPoint 가 없다 
        {
            doorActiveThreshold = thresholdPoint.GetComponent<SavePoint>().achievementNum;
        }      
    }
    void Start() //door 는 localPos 를 기준으로 움직임: 회전은 stageDoor 그룹 자체에 줘야 함 
    {
        initialPos = transform.position; //초기 위치 저장 

        // 가시 스프라이트 및 오프셋 설정 
        if (disposable) //일회성 문일 경우 
        {
            if(doorType == 1) //내려온 상태가 디폴트 
            {
                spr.sprite = spritesGroup[3];
                spikeColl.offset = spikeOffsetGroup[3];
            }

            else if(doorType == 2) //올라간 상태가 디폴트 
            {
                spr.sprite = spritesGroup[0];
                spikeColl.offset = spikeOffsetGroup[0];
            }
        }       
        else //일반적인 메인 스테이지 내에 있는 문
        {
            if (GameManager.instance.gameData.curAchievementNum >= doorActiveThreshold) //만약 이미 열린 문이면 
            {
                if (doorType == 1) //버튼 누르면 올라가는 문 ~> 올라간 채로 있어야 함
                {
                    spr.sprite = spritesGroup[0];
                    transform.position = initialPos + transform.up * doorLength;
                    spikeColl.offset = spikeOffsetGroup[0];
                }
                else //콜라이더 반응하면 내려오는 문 ~> 내려온 채로 있어야 함
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
        //문이 닫힌 상태에서
        //(1) 가시 비활성화 
        //(2) 문 진동
        //(3) 문이 올라감 

        StartCoroutine("spikeDeActive");
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("doorShake");
        yield return new WaitForSeconds(0.6f);

        loopSound.clip = doorOpenLoopSound;
        loopSound.Play();

        float moveSpeed = doorLength / doorPeriod; //문이 올라가는 속도 

        rigid.velocity = transform.up * moveSpeed;

        yield return new WaitForSeconds(doorPeriod - 0.1f);
        playOnceSound.Stop();
        playOnceSound.clip = doorHitSound;
        playOnceSound.Play();

        yield return new WaitForSeconds(0.1f);
        rigid.velocity = Vector3.zero;
        transform.position = initialPos + transform.up * doorLength; //문의 위치 올라간채로 고정 

        loopSound.Stop();
    }

    IEnumerator doorDown()
    {
        //문이 열린 상태에서
        //(1) 문 진동 
        //(2) 문이 내려감
        //(3) 가시 활성화 

        StartCoroutine("doorShake");
        yield return new WaitForSeconds(0.6f);

        loopSound.clip = doorOpenLoopSound;
        loopSound.Play();

        float moveSpeed = doorLength / doorPeriod; //문이 올라가는 속도 

        rigid.velocity = -transform.up * moveSpeed;

        yield return new WaitForSeconds(doorPeriod-0.1f);
        playOnceSound.Stop();
        playOnceSound.clip = doorHitSound;
        playOnceSound.Play();

        yield return new WaitForSeconds(0.1f);

        rigid.velocity = Vector3.zero;
        transform.position = initialPos - transform.up * doorLength; //문의 위치 내려온채로 고정 

        loopSound.Stop();

        yield return new WaitForSeconds(0.5f);
        StartCoroutine("spikeActive");
    }

    IEnumerator spikeActive() //가시 활성화하는 함수 
    {
        for(int index=0; index<4; index++)
        {
            spr.sprite = spritesGroup[index];
            spikeColl.offset = spikeOffsetGroup[index];
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator spikeDeActive() //가시 비활성화시키는 함수 
    {
        for (int index = 3; index >=0; index--)
        {
            spr.sprite = spritesGroup[index];
            spikeColl.offset = spikeOffsetGroup[index];
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator doorShake() //문이 열릴 때, 문이 닫힐 때 흔들림 ~> 0.48초간 흔들림 
    {
        playOnceSound.Stop();
        playOnceSound.clip = doorShakeSound;
        playOnceSound.Play();

        float shakeDegree = 0.03f; //위아래로 흔들림
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
