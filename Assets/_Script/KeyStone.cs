using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyStone : MonoBehaviour
{
    GameObject playerObj;
    
    SpriteRenderer spr;
    public Sprite[] spritesGroup;
    [SerializeField] int spriteIndex;

    //스테이지별로 keyStone 디자인이 다른데 매번 별도의 애니메이터를 만들기 번거로운 관계로 스프라이트 교체를 이용해 연출함
    //sprite 0: idle
    //sprite 0~4: active
    //sprite 4~0: deactive
    //sprite 4 ~> 0: strike

    [SerializeField] bool shouldStoneActive;
    [SerializeField] bool isPlayerInSensor;

    public GameObject keyObj;
    Key keyScript;

    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        playerObj = GameObject.Find("Player").gameObject;
        isPlayerInSensor = false;
        keyScript = keyObj.GetComponent<Key>();

        spr.sprite = spritesGroup[0]; //idle
        spriteIndex = 0;

        if (GameManager.instance.gameData.curAchievementNum >= keyScript.achievementNum)
        {
            //만약 이미 플레이어가 열쇠를 획득한 상황이면 작동하지 않아야 함
            shouldStoneActive = false;
        }
        else
        {
            shouldStoneActive = true;
        }
    }


    void Update()
    {
        if (shouldStoneActive)
        {
            SensorCheck();
        }
    }

    IEnumerator stoneActive() //돌 머리부분이 위로 올라가며 활성화
    {
        while (spriteIndex < 4)
        {
            spriteIndex++;
            spr.sprite = spritesGroup[spriteIndex];

            if (!isPlayerInSensor)
            {
                StartCoroutine("stoneDeActive"); //올라가던 도중 플레이어가 센서 밖으로 나가면 해당 동작 멈추고 비활성화동작 수행
                break;
            }
            yield return new WaitForSeconds(0.1f); //0.5초 후 완전히 올라감
        }
    }

    IEnumerator stoneDeActive() //돌 머리부분이 아래로 내려가며 활성화
    {
        while (spriteIndex > 0)
        {
            spriteIndex--;
            spr.sprite = spritesGroup[spriteIndex];          
            yield return new WaitForSeconds(0.1f); //0.5초 후 완전히 내려옴
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!shouldStoneActive)
        {
            return;
        }

        if(collision.tag == "Player" && transform.eulerAngles.z == playerObj.transform.eulerAngles.z)
        {
            //센서가 플레이어와 닿아 있고 회전각이 같을 때 센서 ON
            isPlayerInSensor = true;
            StartCoroutine("stoneActive");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!shouldStoneActive)
        {
            return;
        }
        isPlayerInSensor = false;
        StartCoroutine("stoneDeActive");
    }

    void SensorCheck()
    {
        if (!isPlayerInSensor)
        {
            return; //플레이어가 센서 내에 있을 때만 작동함
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            spr.sprite = spritesGroup[0]; //strike 

            keyScript.StartCoroutine(keyScript.burst()); //열쇠 먹음 
            shouldStoneActive = false; //한 번 작동했으면 비활성화해야 함
        }
    }
}
