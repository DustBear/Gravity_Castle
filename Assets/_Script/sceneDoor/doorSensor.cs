using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorSensor : MonoBehaviour
{
    GameObject player;
    public GameObject keyIcon;
    public GameObject door;
    bool isPlayerOn;
    bool isDoorOpen;

    SpriteRenderer keySpr;
    [SerializeField] Sprite[] keySprites = new Sprite[5]; //0이 잠긴 자물쇠, 4이 완전히 열린 자물쇠
    [SerializeField] float keyShakeSize; //자물쇠가 흔들리는 정도
    [SerializeField] bool isOntheAction; //현재 키 흔들리거나 부서지는 애니메이션이 진행중인지 

    Vector3 keyInitialPos;
    void Start()
    {
        player = GameObject.Find("Player");      
        keyIcon.SetActive(false);
        isPlayerOn = false;

        keySpr = keyIcon.GetComponent<SpriteRenderer>();
        keySpr.sprite = keySprites[0];

        keyInitialPos = keyIcon.transform.position;
        isOntheAction = false;

        if (GameManager.instance.gameData.curAchievementNum >= door.GetComponent<Door>().achievementNeeded)
        {
            isDoorOpen = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isPlayerOn) //플레이어가 센서 내에 들어와있는 상태에서 E를 누르면
        {
            if (isDoorOpen) return;

            if (GameManager.instance.gameData.curAchievementNum == door.GetComponent<Door>().achievementNeeded - 1) //진행도상 문을 열어야 하는 상황이면
            {
                if (!isOntheAction)
                {
                    StartCoroutine("keyOpen");
                }
                //door 가 비활성화되면 그 자식 오브젝트인 door sensor 와 스크립트도 같이 비활성화 
            }
            else //진행도상 문을 열 수 없는데 열려고 하면 key 를 흔들어야 함
            {
                StopCoroutine("keyShakeCoroutine"); //이미 진행중이던 shake 동작이 있다면 취소해야 함
                StartCoroutine("keyShakeCoroutine");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (isDoorOpen) return;

            keyIcon.SetActive(true);
            isPlayerOn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (isDoorOpen) return;

            keyIcon.SetActive(false);
            isPlayerOn = false;
        }
    }

    public IEnumerator keyOpen() //sceneDoor을 열 수 있을 때~> 열쇠가 열리고 사라진 후 그 다음 문이 열림
    {
        isOntheAction = true;

        for(int index=0; index<=3; index++) //key icon 열림(0 > 1 > 2 > 3)
        {
            keySpr.sprite = keySprites[index];
            yield return new WaitForSeconds(0.1f);
        }

        keySpr.sprite = keySprites[4];
        yield return new WaitForSeconds(0.3f); //마지막 sprite는 조금 더 길게 유지해야 함 

        keyIcon.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic; //열쇠 풀리고 나면 중력작용 받아서 낙하

        Vector3 addForceDir;
        addForceDir = keyIcon.transform.up.normalized;

        keyIcon.gameObject.GetComponent<Rigidbody2D>().AddForce(addForceDir*4, ForceMode2D.Impulse);
        for(int index=1; index<=5; index++) //열쇠 회전
        {
            keyIcon.transform.Rotate(0, 0, 9f);
            yield return new WaitForSeconds(0.02f);
        }
        
        door.GetComponent<Door>().shouldOpen = true; //문 열림 
        yield return new WaitForSeconds(1f);
        keyIcon.SetActive(false); //key icon 끔
    }

    public IEnumerator keyShakeCoroutine() //sceneDoor을 열 수 없을 때 
    {
        keyIcon.transform.position = keyInitialPos; //키 위치 초기화

        for(int index=3; index>=1; index--)
        {
            //0.2초간 1회 좌우왕복

            /*
            keyIcon.transform.Rotate(0, 0, 10f * index); //10n 도 회전 
            yield return new WaitForSeconds(0.05f);
            keyIcon.transform.Rotate(0, 0, -10f * index); //-10n 도 회전 
            yield return new WaitForSeconds(0.05f);
            keyIcon.transform.Rotate(0, 0, -10f * index); //-10n 도 회전 
            yield return new WaitForSeconds(0.05f);
            keyIcon.transform.Rotate(0, 0, 10f * index); //10n 도 회전 
            yield return new WaitForSeconds(0.05f);*/

            keyIcon.transform.Translate(new Vector3(keyShakeSize * index, 0, 0));
            yield return new WaitForSeconds(0.05f);
            keyIcon.transform.Translate(new Vector3(-keyShakeSize * index, 0, 0));
            yield return new WaitForSeconds(0.05f);
            keyIcon.transform.Translate(new Vector3(-keyShakeSize * index, 0, 0));
            yield return new WaitForSeconds(0.05f);
            keyIcon.transform.Translate(new Vector3(keyShakeSize * index, 0, 0));
            yield return new WaitForSeconds(0.05f);
        }
    }
}
