using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevatorCage : MonoBehaviour
{
    public int purposePoint; //엘리베이터가 현재 목표로 하는 지점의 번호
    public bool isAchieved; //엘리베이턱 목표 지점에 도달했는지의 여부 
    //1이면 Pos1, 2이면 Pos2

    [SerializeField] Vector2 pos1;
    [SerializeField] Vector2 pos2;
    //주의: 항상 로컬 위치 기준으로 위쪽 점을 pos1으로 해야 함 
    [SerializeField] float elevatorSpeed;
    [SerializeField] float achieveNumNeeded; //엘리베이터 활성화를 위해 필요한 진척도(이 숫자 '이상'일 때 엘리베이터 이용가능)
    [SerializeField] int initialPos; //시작할 때 위치할 지점 ~> 1이면 Pos1, 2이면 Pos2 가 된다

    [SerializeField] bool isPlayerOnBell; //플레이어가 벨 센서 내에 있는지의 여부 
    [SerializeField] float playerAddforce; 
    //엘리베이터가 아래로 내려갈 때 급출발하기 때문에 플레이어가 공중에 붕 뜨게 됨 ~> 별도의 힘을 순간적으로 줘서 바닥에 붙어있게 하자 

    Rigidbody2D rigid;
    public GameObject weight; //무게추 ~> 늘 두 좌표 중심에 대해 엘리베이터와 반대로 움직여야 함
    public GameObject bell; //벨을 울릴 때마다 벨이 좌우로 흔들려야 함
    public GameObject gear;
    public Animator gearAni;

    Quaternion initialBellRotation; //시작 시 벨의 회전각: 엘리베이터가 옆으로 누워있는 경우에도 벨은 로컬포지션에 대해 회전해야 함 

    private void Awake()
    {
        //시작하면서 엘리베이터 위치 초기화
        if (initialPos == 1)
        {
            transform.position = pos1;
            purposePoint = 1;
        }
        else
        {
            transform.position = pos2;
            purposePoint = 2;
        }

        isAchieved = true; 
        //맨 처음 시작할 때는 purposPoint와 현재 위치가 동일하도록 맞춰 줘야 함 
    }

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        gearAni = gear.GetComponent<Animator>();
        initialBellRotation = bell.transform.rotation;

        gearAni.SetBool("gearMove", false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && collision.transform.rotation == transform.rotation)
        {
            //플레이어와 엘리베이터의 회전각이 다르면 활성화 안됨
            InputManager.instance.isJumpBlocked = true; //엘리베이터 내에서는 점프 불가능 
            isPlayerOnBell = true;
        }       
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            InputManager.instance.isJumpBlocked = false;
            isPlayerOnBell = false;
        }
    }
    void Update()
    {
        if(GameManager.instance.gameData.curAchievementNum < achieveNumNeeded)
        {
            return; 
            //만약 엘리베이터를 이용하기에 진척도가 충분하지 않으면 명령 무시 
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isPlayerOnBell) bellRing();
            //센서 내에 플레이어가 있는 상태로 상호작용키 누르면 벨 울리면서 purposePoint 바뀜 
        }
        
        elevatorMove();
        weightMove();
    }

    void elevatorMove()
    {
        if (isAchieved) return; //목표 지점에 도달했다면 더이상 움직일 필요 없음

        Vector2 dirVector;
        Vector2 moveDirection;

        if (purposePoint == 1) //현재 pos2 에서 pos1으로 이동하고 있는 중이면 
        {
            dirVector = pos1 - pos2;
            moveDirection = dirVector.normalized; //moveDirection = 움직여야 하는 방향(크기1 벡터로 표현)
            rigid.velocity = moveDirection * elevatorSpeed; //엘리베이터 속도 할당    

            if (vectorJudge())
            {
                gearAni.SetBool("gearMove", false);
                transform.position = pos1; //위치 고정하고 
                rigid.velocity = Vector3.zero; //속도 정지시키고 
                isAchieved = true;
            }
        }
        else //현재 pos1 에서 pos2 으로 이동하고 있는 중이면 
        {
            dirVector = pos2 - pos1;
            moveDirection = dirVector.normalized;
            rigid.velocity = moveDirection * elevatorSpeed; //엘리베이터 속도 할당    

            if (vectorJudge())
            {
                gearAni.SetBool("gearMove", false);
                transform.position = pos2; //위치 고정하고 
                rigid.velocity = Vector3.zero; //속도 정지시키고 
                isAchieved = true;
            }
        }
               
       
    }
  
    void bellRing() 
    {
        isAchieved = false;

        //벨을 울리는 동작이 끝나기 전에 다시 울려도 두 로테이션이 중첩되지 않도록 함 
        StopCoroutine("bellShake");
        bell.transform.rotation = initialBellRotation;
        gearAni.SetBool("gearMove", true); //기어 움직임

        StartCoroutine("bellShake");

        if (purposePoint == 1)
        {
            purposePoint = 2;
            Vector3 addForceDir = GameObject.Find("Player").transform.TransformDirection(transform.up) * (-1); //플레이어.up 의 반대 방향을 가르킨다 
            //플레이어가 튕겨져나가지 않게 힘을 가해줘야 하는 방향/플레이어의 회전각에 따라 달라져야 한다

            GameObject.Find("Player").GetComponent<Rigidbody2D>().AddForce(addForceDir * playerAddforce, ForceMode2D.Impulse);
            //pos1로 이동하던 도중 pos2로 목표 바꿈 ~> 플레이어 공중에 뜨지 않게 잡아줘야 함 

            gearAni.SetFloat("gearSpeed", 2);
        }
        else
        {
            purposePoint = 1;
            gearAni.SetFloat("gearSpeed", -2); //이동방향이 반대면 기어의 회전방향도 반대여야 함 
        }
        //엘리베이터에 달려 있는 벨을 누름 ~> 현재 purposePoint가 1이면 2로, 2이면 1로 바꿔줌 
    }

    IEnumerator bellShake() //벨이 좌우로 흔들리는 애니메이션
    {
        float maxRotate=30;
        float middleRotate=20;
        float minRotate=10;

        float[] bellRotation = new float[3] { maxRotate, middleRotate, minRotate };

        float curRotate = 0;
        for(int a=0; a<=2; a++)
        {
            curRotate = bellRotation[a];
            for(int b=0; b<=3; b++)
            {
                switch (b)
                {
                    case 0:
                        bell.transform.Rotate(0, 0, curRotate);
                        break;
                    case 1:
                        bell.transform.Rotate(0, 0, -curRotate);
                        break;
                    case 2:
                        bell.transform.Rotate(0, 0, -curRotate);
                        break;
                    case 3:
                        bell.transform.Rotate(0, 0, curRotate);
                        break;
                }
                yield return new WaitForSeconds(0.05f);
            }
        }       
    }

    bool vectorJudge() //엘리베이터가 선분 위에 있는지 판단함
    {
        Vector2 pos1Vector = pos1 - new Vector2(transform.position.x, transform.position.y);
        Vector2 pos2Vector = pos2 - new Vector2(transform.position.x, transform.position.y);

        if(Vector2.Dot(pos1Vector, pos2Vector) > 0)
        {
            //엘리베이터로부터 양 극단까지의 벡터를 각각 구한 뒤 내적해서 양수면 작동범위 벗어난 것 ~> 도착했다!
            
            return true; 
        }
        else
        {
            return false;
        }
    }

    void weightMove()
    {
        Vector2 centerPos;
        Vector2 cagePos = new Vector2(transform.position.x, transform.position.y);

        centerPos = (pos1 + pos2) / 2;

        weight.transform.position = 2*centerPos - cagePos;
        //무게추는 항상 엘리베이터 룸과 벡터 중심에서 대칭되는 위치에 있음 
    }
}
