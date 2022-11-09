using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class advancedStageDoor_button : MonoBehaviour
{
    [SerializeField] bool isActived; //버튼이 이미 작동했는지의 여부
   
    public GameObject stageDoor; //이 버튼이 제어할 스테이지 문
    public Sprite[] leverSpriteGroup;

    SpriteRenderer spr;
    bool isPlayerOn = false;
    public GameObject keyIcon;

    AudioSource sound;
    public AudioClip popUpSound;
    public AudioClip rotateSound;
    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        sound = GetComponent<AudioSource>();
    }

    void Start()
    {
        //이 스크립트는 항상 advancedStageDoor 스크립트보다 나중에 실행되어야 함 ~> Start() 로 설정 
        if (stageDoor.GetComponent<advancedStageDoor>().disposable) //disposable 설정된 씬을 시작할 때 항상 비활성화된 문 ~> 늘 button 비활성화해야 함 
        {
            isActived = false;
            spr.sprite = leverSpriteGroup[0];
        }       
        else
        {
            if (GameManager.instance.gameData.curAchievementNum >= stageDoor.GetComponent<advancedStageDoor>().doorActiveThreshold)
            {
                spr.sprite = leverSpriteGroup[leverSpriteGroup.Length-1];
                isActived = true;
            }
            else
            {
                isActived = false;
            }
        }

        keyIcon.SetActive(false);
    }

    
    void Update()
    {
        if (isActived)
        {
            return; //이미 작동한 버튼이면 다시 눌러도 무시해야 함
        }

        if (isPlayerOn && Input.GetKeyDown(KeyCode.E))
        {
            isActived = true;
            stageDoor.GetComponent<advancedStageDoor>().doorMove();
            StartCoroutine(buttonSpin());
        }
    }
  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && collision.transform.up == transform.up)
        {
            isPlayerOn = true;
            if (!isActived)
            {
                sound.Stop();
                sound.clip = popUpSound;
                sound.Play();
                keyIcon.SetActive(true);
            }
        }   
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision.transform.up == transform.up)
        {
            isPlayerOn = false;
            keyIcon.SetActive(false);
        }
    }

    IEnumerator buttonSpin()
    {
        sound.Stop();
        sound.clip = rotateSound;
        sound.Play();

        keyIcon.SetActive(false);
        InputManager.instance.isInputBlocked = true; //레버 돌리는 동안은 플레이어 움직이지 못함 
        for (int index=0; index<leverSpriteGroup.Length; index++)
        {
            spr.sprite = leverSpriteGroup[index];
            yield return new WaitForSeconds(0.06f);
        }

        InputManager.instance.isInputBlocked = false;
    }
}
