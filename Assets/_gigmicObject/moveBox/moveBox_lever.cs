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

    AudioSource[] sounds;

    AudioSource leverSound; //레버 찰칵거리는 작동음
    AudioSource boxSound; //박스 활성화, 비활성화 할 때 에너지 효과음
    AudioSource boxMoveSound; //박스 움직일 때 나는 loop 사운드 

    AudioSource boxAmbienceSound; //박스 주변에서 웅웅거리는 환경음 

    public AudioClip leverAct;
    public AudioClip boxActive;
    public AudioClip boxDeActive;
    public AudioClip boxMove;

    private void Awake()
    {
        rigid = moveStone.GetComponent<Rigidbody2D>();
        rigid.bodyType = RigidbodyType2D.Kinematic;

        spr = GetComponent<SpriteRenderer>();
        boxSpr = moveStone.GetComponent<SpriteRenderer>();
        sounds = GetComponents<AudioSource>();

        leverSound = sounds[0];
        boxSound = sounds[1];

        boxMoveSound = sounds[2];
        boxMoveSound.clip = boxMove;
        boxMoveSound.loop = true;

        boxAmbienceSound = moveStone.GetComponent<AudioSource>();
        boxAmbienceSound.loop = true;

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
        moveSoundCheck();
    }

    void moveSoundCheck()
    {
        if(rigid.velocity.magnitude >= 0.05f)
        {
            if (!boxMoveSound.isPlaying)
            {
                boxMoveSound.Play();
            }
        }
        else
        {
            boxMoveSound.Stop();
        }
    }

    void stoneMove()
    {
        if (!isPlayerOn) return; //플레이어가 센서 안에 없으면 무시 

        if (isPlayerOn && Input.GetKeyDown(KeyCode.E)) //플레이어가 인식범위 내에서 E 누르면 활성화 ~> 비활성화 / 비활성화 ~> 활성화 상태로 만듦 
        {            
            if (!isLeverAct)
            {
                isLeverAct = true;
                leverArrow.SetActive(true);
                rigid.bodyType = RigidbodyType2D.Dynamic;
                rigid.gravityScale = 0;
                InputManager.instance.isInputBlocked = true;
                
                //박스 활성화시면 환경음이 켜지고 + active 사운드 켜짐 
                boxSound.Stop();
                boxSound.clip = boxActive;
                boxSound.Play();

                boxAmbienceSound.Play();
            }
            else
            {
                isLeverAct = false;
                leverArrow.SetActive(false);
                rigid.bodyType = RigidbodyType2D.Kinematic;
                InputManager.instance.isInputBlocked = false;

                //박스 활성화시면 환경음이 꺼지고 + active 사운드 꺼짐 
                boxSound.Stop();
                boxSound.clip = boxDeActive;
                boxSound.Play();

                boxAmbienceSound.Stop();
            }
        }

        if (isLeverAct)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                //레버 활성화 상태에서 좌우 화살표 클릭하면 레버 작동음 들림 
                leverSound.Stop();
                leverSound.clip = leverAct;
                leverSound.Play();
            }

            if (Input.GetAxisRaw("Horizontal") == -1) // [<--] 누르면 ~> 값이 작아지는 방향으로 움직임 
            {
                spr.sprite = spriteGroup[1];

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
                    if (rigid.velocity.magnitude < maxSpeed) //최대속도에 도달했다면 더 이상 힘 가하지 않음 
                    {
                        rigid.AddForce(new Vector2(-1, 0) * accelSpeed, ForceMode2D.Impulse);
                        if (!isReClockCorWorking) StartCoroutine("gear_reClockWise"); //반시계방향 회전 시작 
                    }                   
                }
                else //수직이동 
                {
                    if (rigid.velocity.magnitude < maxSpeed)
                    {
                        rigid.AddForce(new Vector2(0, -1) * accelSpeed, ForceMode2D.Impulse);
                        if (!isReClockCorWorking) StartCoroutine("gear_reClockWise"); //반시계방향 회전 시작 
                    }
               }
            }
            else if (Input.GetAxisRaw("Horizontal") == 1) // [-->] 를 누르면 ~> 값이 커지는 방향으로 움직임 
            {
                spr.sprite = spriteGroup[2];

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
                    if (rigid.velocity.magnitude < maxSpeed)
                    {
                        rigid.AddForce(new Vector2(1, 0) * accelSpeed, ForceMode2D.Impulse);
                        if (!isClockCorWorking) StartCoroutine("gear_clockWise"); //시계방향 회전 시작 
                    }                  
                }
                else
                {
                    if (rigid.velocity.magnitude < maxSpeed)
                    {
                        rigid.AddForce(new Vector2(0, 1) * accelSpeed, ForceMode2D.Impulse);
                        if (!isClockCorWorking) StartCoroutine("gear_clockWise"); //시계방향 회전 시작 
                    }                    
                }                
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
