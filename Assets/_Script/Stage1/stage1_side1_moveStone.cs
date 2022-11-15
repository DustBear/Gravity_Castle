using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stage1_side1_moveStone : MonoBehaviour
{
    [SerializeField] float moveTime; //stone이 목적지에 도달하는 데 걸리는 시간
    float moveSpeed; //이동거리를 moveTime으로 나눈 값 

    [SerializeField] float limitSpeed; //떨어질 때 낙하속도 상한 
    [SerializeField] float floatTime; //stone이 최고점에 도달하고 정지해 있는 시간

    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 finishPos;

    public bool shouldStoneStart;
    public bool shouldStoneMove;
    public bool shouldStoneFall;
    public Sprite[] spriteGroup; //[0]은 불 꺼진 상태 [1]~[14] 은 불이 서서히 들어오는 상태(14 frame) 
    Rigidbody2D rigid;

    float fallDelay; //stone이 떨어진 후 걸린 시간 
    SpriteRenderer spr;
   
    public AudioSource sound;
    public AudioSource loopSound;

    public AudioClip act_ready;
    public AudioClip lightOn;
    public AudioClip act_start;
    public AudioClip moving;
    public AudioClip arrive;
    public AudioClip blink_turnOn;
    public AudioClip blink_turnOff;
    public AudioClip windBlow;
    public AudioClip crush;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();

        shouldStoneStart = false;
        shouldStoneMove = false;

        moveTime = moveTime - 0.82f;
        moveSpeed = (finishPos - startPos).magnitude / moveTime; //moveSpeed는 moveTime에 따라 달라진다 

        spr.sprite = spriteGroup[0]; //불 꺼진 상태 
    }

    void Update()
    {
        fallDelay += Time.deltaTime;
        if (shouldStoneStart)
        {
            StartCoroutine(stoneStart());
        }       
    }

    IEnumerator stoneStart()
    {
        shouldStoneStart = false;

        Vector3 vibrateDir = (startPos - finishPos).normalized;
        sound.PlayOneShot(act_ready);

        //stone이 움직이기 전 위 아래로 살짝 진동하면서 깜빡임 
        transform.position += vibrateDir * 0.05f;
        spr.sprite = spriteGroup[spriteGroup.Length - 1];
        sound.PlayOneShot(lightOn);
        yield return new WaitForSeconds(0.08f);

        transform.position -= vibrateDir * 0.05f;
        yield return new WaitForSeconds(0.08f);

        transform.position -= vibrateDir * 0.05f;
        spr.sprite = spriteGroup[0];
        yield return new WaitForSeconds(0.08f);

        transform.position += vibrateDir * 0.05f;
        yield return new WaitForSeconds(0.08f);

        yield return new WaitForSeconds(0.5f); //레버를 당기고 0.82초가 지나야 돌이 움직이기 시작

        //stone 진동이 끝나면 목표지점으로 움직이기 시작 
        shouldStoneMove = true;

        sound.PlayOneShot(act_start);

        loopSound.clip = moving;
        loopSound.Play();

        while (shouldStoneMove)
        {
            stoneMove();            
            yield return null;
        }
        
    }

    float spriteTimer = 0; //stone이 움직이는 동안 타이머가 돌아가며 진행도에 맞춰 sprite 바꿔줌 
    int spriteIndex = 1;

    void stoneMove()
    {        
        spriteTimer += Time.deltaTime;
        rigid.velocity = (finishPos - startPos).normalized * moveSpeed;

        if(spriteTimer >= moveTime / (spriteGroup.Length - 1))
        {
            if(spriteIndex < spriteGroup.Length - 1)
            {
                spriteIndex++;
                spr.sprite = spriteGroup[spriteIndex];
                spriteTimer = 0; //타이머 초기화 
            }            
        }

        Vector3 toStartPos = startPos - transform.position;
        Vector3 toFinishPos = finishPos - transform.position;

        if((Vector3.Dot(toStartPos,toFinishPos) > 0) && rigid.velocity.magnitude > 0) 
            //stone으로부터 finishPos 까지의 벡터의 방향이 stone-startPos 벡터의 방향과 같고 stone의 속도가 0이상이면 목표지점에 도착한것으로 봄
        {
            spriteTimer = 0;
            shouldStoneMove = false;
            rigid.velocity = Vector3.zero;
            transform.position = finishPos;
            spriteIndex = 1;

            sound.PlayOneShot(arrive);
            loopSound.Stop();
            StartCoroutine("stoneFloat");
        }
    }
    
    IEnumerator stoneFloat() //stone이 공중에 머무른 상태에서 잠시 머무르다가 떨어짐
    {
        for (int index = 1; index <= 4; index++) //4번 깜빡인 다음 낙하 
        {
            yield return new WaitForSeconds(floatTime / 8);
            spr.sprite = spriteGroup[0];
            sound.PlayOneShot(blink_turnOff);

            yield return new WaitForSeconds(floatTime / 8);
            spr.sprite = spriteGroup[spriteGroup.Length-1];
            sound.PlayOneShot(blink_turnOn);
        }

        shouldStoneFall = true;
        fallDelay = 0; //타이머 초기화 

        loopSound.clip = windBlow;
        loopSound.Play();

        while (shouldStoneFall)
        {
            stoneFall();
            yield return null;
        }
    }
 
    void stoneFall()
    {
        int gravityScale = 3;
        if (rigid.velocity.y <= -limitSpeed) //속도 상한에 도달하면 속도 고정 
        {
            rigid.velocity = -(finishPos - startPos).normalized * limitSpeed;
        }
        else
        {
            float fallSpeed = gravityScale * 4.9f * fallDelay*fallDelay; //가속도 9.8인 등가속도운동(1/2*t^2에 비례)
            rigid.velocity = -(finishPos - startPos).normalized * fallSpeed;
        }
        
        
        Vector3 toStartPos = startPos - transform.position;
        Vector3 toFinishPos = finishPos - transform.position;

        if ((Vector3.Dot(toStartPos, toFinishPos) > 0) && rigid.velocity.magnitude > 0)
        //stone으로부터 finishPos 까지의 벡터의 방향이 stone-startPos 벡터의 방향과 같고 stone의 속도가 0이상이면 목표지점에 도착한것으로 봄
        {
            shouldStoneFall = false;
            rigid.velocity = Vector3.zero;
            transform.position = startPos;
            spr.sprite = spriteGroup[0];

            loopSound.Stop();
            sound.PlayOneShot(crush);
            UIManager.instance.cameraShake(0.3f, 0.4f);            
        }
    }
}
