using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startScene_circleDoor : MonoBehaviour
{
    public GameObject playerObj;
    SpriteRenderer spr;
    SpriteRenderer thisSpr;

    public float doorMoveLength;
    public float doorMoveDelay;

    public Sprite[] doorRotateSprites;
    public Sprite[] doorOpenSprites;

    public GameObject[] firstInform; //맨 처음 좌우조작키 설명은 문이 완전히 열린 뒤 떠야 함 
    private void Awake()
    {
        spr = playerObj.GetComponent<SpriteRenderer>();
        thisSpr = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        if(GameManager.instance.gameData.curAchievementNum == 0) //해당 세이브파일을 처음 시작할 때만 사용 
        {
            StartCoroutine(doorOpenScene());
            for (int index = 0; index < firstInform.Length; index++)
            {
                firstInform[index].SetActive(false);
            }
        }
        else
        {
            transform.position += new Vector3(0, doorMoveLength, 0); //처음 시작한 것이 아니면 문은 올라가있어야 함 
            for (int index = 0; index < firstInform.Length; index++)
            {
                firstInform[index].SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator doorOpenScene()
    {        
        spr.sortingLayerName = "stageStartPlayer"; //처음에는 플레이어가 문 뒤에 있어야 함 
        InputManager.instance.isInputBlocked = true; //플레이어 움직이지 못하게 함 

        UIManager.instance.FadeIn(4f); //4초에 걸쳐 화면 밝아짐 
        yield return new WaitForSeconds(7f);

        for (int index = 0; index < 3; index++) //잠금장치 작동직전 진동 
        {
            transform.position += new Vector3(0, 1, 0) * 0.06f;
            yield return new WaitForSeconds(0.06f);
            transform.position -= new Vector3(0, 1, 0) * 0.06f;
            yield return new WaitForSeconds(0.06f);          
        }
        yield return new WaitForSeconds(1f);

        for (int num=0; num<=3; num++) //잠금장치가 4회 회전 
        {
            for (int index = 0; index <= doorRotateSprites.Length - 1; index++)
            {
                thisSpr.sprite = doorRotateSprites[index];
                yield return new WaitForSeconds(0.35f);
            }
        }
        thisSpr.sprite = doorRotateSprites[0];

        yield return new WaitForSeconds(1f);

        for (int index = 0; index <= doorOpenSprites.Length - 2; index++)
        {
            thisSpr.sprite = doorOpenSprites[index];
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(2f);

        for (int index=0; index<3; index++) //문 올라가기 직전 진동함 
        {
            transform.position += new Vector3(0, 1, 0) * 0.06f;
            yield return new WaitForSeconds(0.06f);
            transform.position -= new Vector3(0, 1, 0) * 0.06f;
            yield return new WaitForSeconds(0.06f);           
        }
        yield return new WaitForSeconds(1f);

        for (int index=0; index<200; index++) //문 올라감 
        {
            transform.position += new Vector3(0, 1, 0) * (doorMoveLength / 200);
            yield return new WaitForSeconds(doorMoveDelay / 200);
        }

        spr.sortingLayerName = "Player"; //플레이어가 문 앞으로 옴 
        yield return new WaitForSeconds(1f);

        textShow();
        InputManager.instance.isInputBlocked = false; //플레이어 움직일 수 있음 
    }

    void textShow()
    {
        for(int index=0; index<firstInform.Length; index++)
        {
            firstInform[index].SetActive(true);
        }
    }
}
