using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveBox_lever : MonoBehaviour
{
    public GameObject moveStone;

    Rigidbody2D rigid;
    SpriteRenderer spr;
    SpriteRenderer boxSpr;

    public Sprite[] spriteGroup; //[0]=idle  [1]=left  [2]=right
    public Sprite[] boxSprites; //움직이는 moveBox의 sprite 
    [SerializeField] int curSpriteIndex;

    public float accelSpeed; //가속도 
    public float maxSpeed;

    [SerializeField] bool isPlayerOn;
    [SerializeField] bool isLeverAct;

    bool isMoveHorizontal; //true 이면 box 가 가로로 움직임 

    //pos1의 수치가 pos2 보다 커야 함(x든 y든 둘 다 해당)
    public Vector2 pos1;
    public Vector2 pos2;

    public GameObject leverArrow;
   
    bool isClockCorWorking = false;
    bool isReClockCorWorking = false;

    IEnumerator clockCor;
    IEnumerator reclockCor;

    public AudioClip activeSound;
    public AudioClip moveSound;

    AudioSource sound;
    private void Awake()
    {
        rigid = moveStone.GetComponent<Rigidbody2D>();
        rigid.bodyType = RigidbodyType2D.Kinematic;

        spr = GetComponent<SpriteRenderer>();
        boxSpr = moveStone.GetComponent<SpriteRenderer>();
        sound = GetComponent<AudioSource>();

        isPlayerOn = false;
        isLeverAct = false;
    }
    void Start()
    {
        if (pos1.x == pos2.x) //x좌표가 같으면 세로로 움직이는 것 
        {
            isMoveHorizontal = false;
        }
        else
        {
            isMoveHorizontal = true;
        }
        spr.sprite = spriteGroup[0];
        leverArrow.SetActive(false);

        curSpriteIndex = 0;

        clockCor = gear_clockWise();
        reclockCor = gear_reClockWise();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") isPlayerOn = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player") isPlayerOn = false;
        rigid.velocity = Vector3.zero; //플레이어가 레버 센서 밖으로 나가면 stone 도 정지 
        spr.sprite = spriteGroup[0];
    }
    void Update()
    {
        stoneMove();       
    }

    void stoneMove()
    {
        if (!isPlayerOn) return; //플레이어가 센서 안에 없으면 무시 

        if (isPlayerOn && Input.GetKeyDown(KeyCode.E)) //플레이어가 인식범위 내에서 E 누르면 활성화 ~> 비활성화 / 비활성화 ~> 활성화 상태로 만듦 
        {
            sound.Stop();
            sound.clip = activeSound;
            sound.Play();

            if (!isLeverAct)
            {
                isLeverAct = true;
                leverArrow.SetActive(true);
                rigid.bodyType = RigidbodyType2D.Dynamic;
                rigid.gravityScale = 0;
                InputManager.instance.isInputBlocked = true;
            }
            else
            {
                isLeverAct = false;
                leverArrow.SetActive(false);
                rigid.bodyType = RigidbodyType2D.Kinematic;
                InputManager.instance.isInputBlocked = false;
            }
        }

        if (isLeverAct)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                sound.Stop();
                sound.clip = moveSound;
                sound.Play();
            }

            if (Input.GetAxisRaw("Horizontal") == -1) // [<--] 누르면 ~> 값이 작아지는 방향으로 움직임 
            {
                //이미 pos2 좌표에 도달했다면 더 이상 pos2 방향으로는 움직이지 않음 
                if ((isMoveHorizontal && moveStone.transform.position.x <= pos2.x) || (!isMoveHorizontal && moveStone.transform.position.y <= pos2.y))
                {
                    rigid.velocity = Vector3.zero;
                    moveStone.transform.position = pos2;

                    StopCoroutine(reclockCor); //반시계방향 회전 중지 
                    isReClockCorWorking = false;
                    return;
                }

                if (isMoveHorizontal) //수평이동
                {
                    if (rigid.velocity.magnitude >= maxSpeed) //최대속도에 도달했다면 더 이상 힘 가하지 않음 
                    {
                        return;
                    }

                    rigid.AddForce(new Vector2(-1, 0) * accelSpeed, ForceMode2D.Impulse);
                    if (!isReClockCorWorking) StartCoroutine("gear_reClockWise"); //반시계방향 회전 시작 
                }
                else //수직이동 
                {
                    if (rigid.velocity.magnitude >= maxSpeed)
                    {
                        return;
                    }

                    rigid.AddForce(new Vector2(0, -1) * accelSpeed, ForceMode2D.Impulse);
                    if (!isReClockCorWorking) StartCoroutine("gear_reClockWise"); //반시계방향 회전 시작 
                }

                spr.sprite = spriteGroup[1];
            }
            else if (Input.GetAxisRaw("Horizontal") == 1) // [-->] 를 누르면 ~> 값이 커지는 방향으로 움직임 
            {
                //이미 pos1 좌표에 도달했다면 더 이상 pos1 방향으로는 움직이지 않음 
                if ((isMoveHorizontal && moveStone.transform.position.x >= pos1.x) || (!isMoveHorizontal && moveStone.transform.position.y >= pos1.y))
                {
                    rigid.velocity = Vector3.zero;
                    moveStone.transform.position = pos1;

                    StopCoroutine(clockCor); //시계방향 회전 중지 
                    isClockCorWorking = false;
                    return;
                }

                if (isMoveHorizontal)
                {
                    if (rigid.velocity.magnitude >= maxSpeed)
                    {
                        return;
                    }
                    rigid.AddForce(new Vector2(1, 0) * accelSpeed, ForceMode2D.Impulse);
                    if (!isClockCorWorking) StartCoroutine("gear_clockWise"); //시계방향 회전 시작 
                }
                else
                {
                    if (rigid.velocity.magnitude >= maxSpeed)
                    {
                        return;
                    }
                    rigid.AddForce(new Vector2(0, 1) * accelSpeed, ForceMode2D.Impulse);
                    if (!isClockCorWorking) StartCoroutine("gear_clockWise"); //시계방향 회전 시작 
                }

                spr.sprite = spriteGroup[2];
            }
            else //아무런 키도 누르지 않을 때 
            {
                rigid.velocity = Vector3.zero;
                spr.sprite = spriteGroup[0];

                if (isClockCorWorking)
                {
                    StopCoroutine(clockCor);
                    isClockCorWorking = false;
                }
                if (isReClockCorWorking)
                {
                    StopCoroutine(reclockCor);
                    isReClockCorWorking = false;
                }
            }
        }
    }

    IEnumerator gear_clockWise() //시계방향 회전 
    {
        isClockCorWorking = true;
        while (true)
        {
            if (isClockCorWorking == false)
            {
                yield break;
            }
            if (curSpriteIndex == boxSprites.Length - 1)
            {
                curSpriteIndex = 0; //sprite 그룹 끝까지 가면 다시 0으로 돌아옴 
            }
            else
            {
                curSpriteIndex++;
            }

            boxSpr.sprite = boxSprites[curSpriteIndex];
            yield return new WaitForSeconds(0.08f);
        }
    }

    IEnumerator gear_reClockWise() //반시계방향 회전 
    {
        isReClockCorWorking = true;
        while (true)
        {
            if (isReClockCorWorking == false)
            {
                yield break;
            }
            if (curSpriteIndex == 0)
            {
                curSpriteIndex = boxSprites.Length - 1;
            }
            else
            {
                curSpriteIndex--;
            }

            boxSpr.sprite = boxSprites[curSpriteIndex];           
            yield return new WaitForSeconds(0.08f);
        }
    }
}
