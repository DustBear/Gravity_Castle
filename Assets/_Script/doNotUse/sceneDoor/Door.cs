using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] int stageNum;

    public int achievementNeeded;
    
    //문은 직접 세이브포인트의 기능을 하지 않는다 
    //씬을 시작한 시점에서 특정 세이브포인트 성취도 이상일 때 열려 있으며 그 이하일 때는 닫혀 있는다
    
    public bool shouldOpen;

    Animator ani;
    public GameObject sensor;
    public ParticleSystem emitBurst;
    public AnimationClip aniClip; //문 열리는 애니메이션 ~> 애니메이션 길이 측정 위한 파라미터 

    //센서 내부에 플레이어가 들어오면 ~> interactiveText를 활성화시킨다
    //센서 내부에 플레이어가 들어와 있는 상태에서 플레이어가 E를 누르면 문을 열고 interactiveText를 비활성화시킨다 
    //센서 내부에 플레이어가 들어와 있다가 나가면 interactiveText를 비활성화시킨다

    GameObject player;
    SpriteRenderer sprite;
    [SerializeField] float doorColliderForce;

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        sprite = GetComponent<SpriteRenderer>();      
        ani = GetComponent<Animator>();

        shouldOpen = false;
    }

    void Start()
    {
        sensor.SetActive(true); //문이 처음 생성되면 당연히 sensor 도 있어야 함 

        if (GameManager.instance.gameData.curAchievementNum >= achievementNeeded)
        {
            Debug.Log("door open" + GameManager.instance.gameData.curAchievementNum);
            gameObject.SetActive(false); //만약 진행도 상 문이 이미 열려있어야 한다면 비활성화함 
        }
    }

    private void Update()
    {
        if (shouldOpen)
        {
            doorOpen();
        }      
    }

    void doorOpen()
    {
        if (GameManager.instance.gameData.curAchievementNum == achievementNeeded - 1)
        {            
            ani.SetBool("shouldDoorOpen", true); //문 열기 애니메이션 실행

            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        /*
        if (achievementNum == 33 && !GameManager.instance.isCliffChecked)
        {
            InputManager.instance.isInputBlocked = true;
        }
        */

        float aniLength = aniClip.length;
        
        yield return new WaitForSeconds(aniLength); //문 여는 애니메이션이 다 끝나고 나서야 작동
       
        emitBurst.Play();
        gameObject.SetActive(false); //끝나면 문 비활성화            
    }

    
}
