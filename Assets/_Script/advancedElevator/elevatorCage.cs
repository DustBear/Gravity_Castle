using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevatorCage : MonoBehaviour
{
    public int purposePoint; 
    //엘리베이터가 현재 목표로 하는 지점의 번호
    //1이면 Pos1, 2이면 Pos2

    public bool isAchieved; //엘리베이턱 목표 지점에 도달했는지의 여부 

    public Vector2 pos1;
    public Vector2 pos2;
    //주의: 항상 '로컬 위치 기준'으로 위쪽 점을 pos1으로 해야 함 
    [SerializeField] float elevatorSpeed;   
    [SerializeField] int initialPos; //시작할 때 위치할 지점 ~> 1이면 Pos1, 2이면 Pos2 가 된다

    [SerializeField] public bool isPlayerOnBell; //플레이어가 벨 센서 내에 있는지의 여부 
    [SerializeField] float playerAddforce; 
    //엘리베이터가 아래로 내려갈 때 급출발하기 때문에 플레이어가 공중에 붕 뜨게 됨 ~> 별도의 힘을 순간적으로 줘서 바닥에 붙어있게 하자 

    Rigidbody2D rigid;
    public GameObject weight; //무게추 ~> 늘 두 좌표 중심에 대해 엘리베이터와 반대로 움직여야 함
    public GameObject bell; //벨을 울릴 때마다 벨이 좌우로 흔들려야 함
    public GameObject gear;
    public Animator gearAni;

    //AudioSource soundPlayer;
    //AudioSource bellSoundPlayer;   

    Quaternion initialBellRotation; //시작 시 벨의 회전각: 엘리베이터가 옆으로 누워있는 경우에도 벨은 로컬포지션에 대해 회전해야 함 
    [SerializeField] Vector3 addForceDir; //엘리베이터의 각도에 따라 플레이어에게 힘을 가하는 방향도 달라져야 함 

    AudioSource bellSound;
    AudioSource elevatorSound;//출발, 도착 사운드 
    AudioSource elevatorLoopSound;

    public AudioClip bellEffect;
    public AudioClip elevatorDepart;
    public AudioClip elevatorEffect;
    public AudioClip elevatorArrive;

    public GameObject E_icon;

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

        //soundPlayer = GetComponent<AudioSource>();
        //bellSoundPlayer = bell.GetComponent<AudioSource>();
        isAchieved = true;

        //맨 처음 시작할 때는 purposePoint와 현재 위치가 동일하도록 맞춰 줘야 함 

        bellSound = gameObject.AddComponent<AudioSource>();
        bellSound.loop = false;
        bellSound.playOnAwake = false;
        bellSound.clip = bellEffect;

        elevatorSound = gameObject.AddComponent<AudioSource>();       
        elevatorSound.playOnAwake = false;
        elevatorSound.loop = false;

        elevatorLoopSound = gameObject.AddComponent<AudioSource>();
        elevatorLoopSound.playOnAwake = false;
        elevatorLoopSound.loop = true;
        elevatorLoopSound.clip = elevatorEffect;
    }

    void Start()
    {
        bellSound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
        elevatorSound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
        elevatorLoopSound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;

        rigid = GetComponent<Rigidbody2D>();
        gearAni = gear.GetComponent<Animator>();
        initialBellRotation = bell.transform.rotation;

        gearAni.SetBool("gearMove", false);
        StartCoroutine(soundCheck());

        E_icon.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && collision.transform.up == transform.up)
        {
            //플레이어와 엘리베이터의 회전각이 다르면 활성화 안됨 
            E_icon.SetActive(true);
            isPlayerOnBell = true;
        }       
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            E_icon.SetActive(false);
            isPlayerOnBell = false;           
        }
    }
    void Update()
    {       
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isPlayerOnBell) bellRing();
            //센서 내에 플레이어가 있는 상태로 상호작용키 누르면 벨 울리면서 purposePoint 바뀜 
            E_icon.SetActive(false);
        }

        elevatorMove();
        weightMove();
    }

    
    IEnumerator soundCheck()
    {
        while (true)
        {
            if (rigid.velocity.magnitude >= 0.05f) //엘리베이터가 움직이는 중이면 
            {
                if (!elevatorLoopSound.isPlaying) //만약 움직이는 사운드가 재생중이지 않으면 
                {
                    elevatorLoopSound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
                    elevatorLoopSound.Play();
                }
            }
            else
            {
                if (elevatorLoopSound.isPlaying)
                {
                    yield return new WaitForSeconds(0.1f); 
                    //이동 사운드를 바로 중단시키면 도착 사운드와 이동 사운드 사이에 틈이 생겨 부자연스러움
                    //0.1초만 딜레이를 줌 
                    elevatorLoopSound.Stop();
                }
            }

            yield return null; //매 프레임마다 실행 
        }        
    }
    
    void elevatorMove()
    {
        if (isAchieved)
        {                                        
            return; //목표 지점에 도달했다면 더이상 움직일 필요 없음
        }
            
        Vector2 dirVector;
        Vector2 moveDirection;

        if (purposePoint == 1) //현재 pos2 에서 pos1으로 이동하고 있는 중이면 
        {           
            gearAni.SetBool("gearMove", true);
            gearAni.SetFloat("gearSpeed", 2f);

            dirVector = pos1 - pos2;
            moveDirection = dirVector.normalized; //moveDirection = 움직여야 하는 방향(크기1 벡터로 표현)
            rigid.velocity = moveDirection * elevatorSpeed; //엘리베이터 속도 할당    

            if (vectorJudge())
            {
                elevatorSound.Stop();
                elevatorSound.clip = elevatorArrive;

                elevatorSound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
                elevatorSound.Play();

                gearAni.SetBool("gearMove", false);
                transform.position = pos1; //위치 고정하고 
                rigid.velocity = Vector3.zero; //속도 정지시키고 
                isAchieved = true;
            }
        }
        else //현재 pos1 에서 pos2 으로 이동하고 있는 중이면 
        {           
            gearAni.SetBool("gearMove", true);
            gearAni.SetFloat("gearSpeed", -2f);

            dirVector = pos2 - pos1;
            moveDirection = dirVector.normalized;
            rigid.velocity = moveDirection * elevatorSpeed; //엘리베이터 속도 할당    

            if (vectorJudge())
            {
                elevatorSound.Stop();
                elevatorSound.clip = elevatorArrive;

                elevatorSound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
                elevatorSound.Play();

                gearAni.SetBool("gearMove", false);
                transform.position = pos2; //위치 고정하고 
                rigid.velocity = Vector3.zero; //속도 정지시키고 
                isAchieved = true;
            }
        }
               
       
    }
  
    void bellRing() 
    {
        if (isAchieved) //정지해 있던 상태에서 벨 울릴 때 
        {
            elevatorSound.Stop();
            elevatorSound.clip = elevatorDepart;

            elevatorSound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
            elevatorSound.Play();
        }

        isAchieved = false;
        bellSound.Stop();

        bellSound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
        bellSound.Play();

        /*
        if (bellSoundPlayer.isPlaying) bellSoundPlayer.Stop();
        bellSoundPlayer.Play();
        */

        //벨을 울리는 동작이 끝나기 전에 다시 울려도 두 로테이션이 중첩되지 않도록 함 
        StopCoroutine("bellShake");
        bell.transform.rotation = initialBellRotation;       
        StartCoroutine("bellShake");

        if (purposePoint == 1)
        {
            purposePoint = 2;           
            GameObject.Find("Player").GetComponent<Rigidbody2D>().AddForce(addForceDir * playerAddforce, ForceMode2D.Impulse);
            //pos1로 이동하던 도중 pos2로 목표 바꿈 ~> 플레이어 공중에 뜨지 않게 잡아줘야 함 
        }
        else
        {
            purposePoint = 1;         
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
