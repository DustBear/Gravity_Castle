using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class SavePoint : MonoBehaviour
{
    GameObject cameraObj;
    [SerializeField] int stageNum; 
    public int achievementNum; 

    Transform player;
    public Vector2 respawnPos; //리스폰 위치 
    public Vector2 respawnDir; //리스폰 방향

    bool isPlayerOnSensor;
    bool isSavePointActivated;

    SpriteRenderer spr;
    public Sprite[] spriteGroup;
    /* [0] : 비활성화 상태(불 꺼짐)
     * [1]~[9] : 활성화 단계
     * [10] : 활성화가 끝난 상태 
     */
    int lastIndex; //spriteGroup 의 길이 
    [SerializeField] int curSpriteNum; //현재 sprite 번호 

    IEnumerator curCoroutine;
    bool isCorWorking;

    public ParticleSystem rightBurst;
    public ParticleSystem leftBurst;

    AudioSource sound;

    public AudioClip lightOnSound;
    public AudioClip lightOffSound;
    public AudioClip activeSound;

    public ParticleSystem particle;
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spr = GetComponent<SpriteRenderer>();
        cameraObj = GameObject.FindWithTag("MainCamera");
        sound = GetComponent<AudioSource>();

        respawnPos = transform.position; //세이브포인트의 위치가 이 세이브를 이용할 때 리스폰돼야 하는 위치   
        lastIndex = spriteGroup.Length - 1;
    }

    private void Start()
    {
        if (GameManager.instance.gameData.savePointUnlock[GameManager.instance.saveNumCalculate(new Vector2(stageNum, achievementNum))] == 1) 
        {
            spr.sprite = spriteGroup[lastIndex]; //활성화 상태
            curSpriteNum = lastIndex;
            particle.Play();
            isSavePointActivated = true;
        }
        else
        {
            spr.sprite = spriteGroup[0]; //비활성화 상태 
            curSpriteNum = 0;
            isSavePointActivated = false;
        }
    }

    private void Update()
    {
        //원래 이미 활성화된 세이브포인트도 원하면 다시 활성화할 수 있어야 함 
        //ex) 이미 클리어한 스테이지를 다시 돌아와서 할 때 
        if(isSavePointActivated && isPlayerOnSensor && Input.GetKeyDown(KeyCode.E))
        {           
            StartCoroutine(reSaveData());
        }

        //if (isSavePointActivated) return; //활성화된 이후에는 따로 작동x 
        if(!isSavePointActivated && isPlayerOnSensor && Input.GetKeyDown(KeyCode.E)) //처음으로 세이브포인트를 활성화시킴 
        {           
            StartCoroutine(SaveData());
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {       
        if (collision.CompareTag("Player") && transform.up == player.transform.up) 
        {
            //플레이어가 세이브포인트에 닿아 있고 각도가 같을 때 저장 가능 
            isPlayerOnSensor = true;
            if (!isSavePointActivated) //불 켜지고 꺼지는 애니메이션은 비활성화된 세이브에서만 작동 
            {
                if (isCorWorking)
                {
                    StopCoroutine(curCoroutine);
                    isCorWorking = false;
                }
                curCoroutine = lightOn();
                StartCoroutine(curCoroutine);
            }           
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerOnSensor = false;
            if (!isSavePointActivated)
            {
                if (isCorWorking)
                {
                    StopCoroutine(curCoroutine);
                    isCorWorking = false;
                }
                curCoroutine = lightOff();
                StartCoroutine(curCoroutine);
            }           
        }
    }

    IEnumerator lightOn()
    {
        isCorWorking = true;

        sound.Stop();
        sound.clip = lightOnSound;
        sound.Play();

        for(int index=curSpriteNum; index<=lastIndex-1; index++)
        {
            spr.sprite = spriteGroup[index];
            curSpriteNum = index;
            yield return new WaitForSeconds(0.03f);
        }
    }

    IEnumerator lightOff()
    {
        isCorWorking = true;

        sound.Stop();
        sound.clip = lightOffSound;
        sound.Play();

        for (int index=curSpriteNum; index>=0; index--)
        {
            spr.sprite = spriteGroup[index];
            curSpriteNum = index;
            yield return new WaitForSeconds(0.03f);
        }
    }

    IEnumerator SaveData()
    {
        if (GameManager.instance.gameData.collectionTmp.Count != 0) //임시저장 리스트에 하나라도 수집요소가 있으면 
        {
            UIManager.instance.collectionAlarm(2); //탐험가상자를 저장했다는 메시지 출력 
        }

        isSavePointActivated = true;

        GameManager.instance.SaveData(achievementNum, stageNum, respawnPos);

        StopCoroutine(curCoroutine);

        spr.sprite = spriteGroup[lastIndex];
        curSpriteNum = lastIndex;

        rightBurst.Play();
        leftBurst.Play();

        sound.Stop();
        sound.clip = activeSound;
        sound.Play();

        UIManager.instance.cameraShake(0.3f, 0.5f); //세이브스톤이 박힐 때 카메라 진동 
        particle.Play();
        yield return null;
    }

    IEnumerator reSaveData() //이미 활성화한 세이브를 다시 활성화 ~> stone이 느리게 상승했다가 다시 하강 
    {
        if(GameManager.instance.gameData.collectionTmp.Count != 0) //임시저장 리스트에 하나라도 수집요소가 있으면 
        {
            UIManager.instance.collectionAlarm(2); //탐험가상자를 저장했다는 메시지 출력 
        }
        
        GameManager.instance.SaveData(achievementNum, stageNum, respawnPos);

        for (int index = lastIndex; index >=0; index--)
        {
            spr.sprite = spriteGroup[index];
            yield return new WaitForSeconds(0.03f);
        }

        yield return new WaitForSeconds(0.2f); 
        
        for (int index = 1; index <= lastIndex; index++)
        {
            spr.sprite = spriteGroup[index];
            yield return new WaitForSeconds(0.03f);
        }

        rightBurst.Play();
        leftBurst.Play();

        sound.Stop();
        sound.clip = activeSound;
        sound.Play();

        UIManager.instance.cameraShake(0.3f, 0.5f);
        //세이브스톤이 박힐 때 카메라 진동 
    }
}
