using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevatorLever : MonoBehaviour
{
    public GameObject advancedElevator;
    elevatorCage elevatorScript;
    SpriteRenderer spr;

    [SerializeField] int leverPos; //이 레버가 pos1, pos2 중 어디로 오게 만드는 레버인지 인식 
    [SerializeField] bool isPlayerOn; //플레이어가 지금 레버를 작동시킬 수 있는 위치에 있는가

    public Sprite[] leverSprite;
    AudioSource leverSound;
    void Start()
    {
        elevatorScript = advancedElevator.GetComponent<elevatorCage>();
        spr = GetComponent<SpriteRenderer>();
        spr.sprite = leverSprite[0];
        leverSound = GetComponent<AudioSource>();
    }
 
    void Update()
    {
        if(isPlayerOn && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine("spriteAni"); //작동여부에 상관없이 레버 애니메이션은 작동

            Vector2 elePos = advancedElevator.transform.position;
            float dis2Pos1 = (elePos - elevatorScript.pos1).magnitude;
            float dis2Pos2 = (elePos - elevatorScript.pos2).magnitude;

            if (leverPos == 1 && dis2Pos1>0.1f) //이미 엘리베이터가 pos1에 도달한 상황에선 작동 x
            {               
                elevatorScript.purposePoint = 1;
                elevatorScript.isAchieved = false;
            }

            else if(leverPos == 2 && dis2Pos2>0.1f) //이미 엘리베이터가 pos2에 도달한 상황에선 작동 x
            {               
                elevatorScript.purposePoint = 2;
                elevatorScript.isAchieved = false;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && collision.transform.up == transform.up)
        {
            isPlayerOn = true;
        }       
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isPlayerOn = false;
        }
    }

    IEnumerator spriteAni() //스프라이트 움직임으로 애니메이션 구현
    {
        leverSound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
        leverSound.Play();
        spr.sprite = leverSprite[1];
        yield return new WaitForSeconds(0.4f);

        spr.sprite = leverSprite[0];
    }
}
