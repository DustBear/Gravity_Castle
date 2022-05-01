using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openingSceneDoor : MonoBehaviour
{
    public float startYpos;
    public float finishYpos;
    public float doorSpeed;
    public float delayTime;
    public GameObject player;
    public float colorValue;
    public bool isColorBrighten;
    float curTime;
    public bool isDoorMove;
    
    public Collider2D[] coll = new Collider2D[3];
    SpriteRenderer spr;
    Rigidbody2D rigid;
    void Start()
    {
        spr = player.GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();

        if (player.transform.position.y <= -8) //씬이 처음 시작하는 것이 아니면 플레이어 sortingLayer, color 무시해야 함 
        {
            spr.sortingLayerID = SortingLayer.NameToID("stageStartPlayer"); //시작하면 플레이어는 엘리베이터 뒤 레이어에서 시작해야 함
            spr.color = new Color(colorValue, colorValue, colorValue); //시작하면 플레이어 색은 어두움
            transform.localPosition = new Vector2(0, startYpos);
        }
        else
        {
            transform.localPosition = new Vector2(0, finishYpos); //플레이어가 씬을 처음시작한 게 아니면 문은 올라간 채로 있어야 함 
        }
        
        isDoorMove = false;
    }

    void Update()
    {
        if (isDoorMove) 
        {
            StartCoroutine(doorMove()); 
        }
    
        if (transform.localPosition.y >= finishYpos)
        {
            transform.localPosition = new Vector2(0, finishYpos); 
            isDoorMove = false;

            spr.sortingLayerID = SortingLayer.NameToID("Player"); //콜라이더 사라지면 레이어 다시 player 로 바꿔줌

            for (int index = 0; index < 3; index++)
            {
                coll[index].gameObject.SetActive(false); //collider는 문이 도착하고 난 이후에 사라짐                
            }
        }

        if (isColorBrighten)
        {
            float tmpColor = spr.color.r;
            spr.color = new Color(tmpColor+ 0.01f, tmpColor + 0.01f, tmpColor + 0.01f); //20프레임 이후 완전히 밝아짐

            if(tmpColor >= 1)
            {
                isColorBrighten = false; //색이 1이 되고 나면 더이상 밝아질 필요 없음
            }
        }
    }

    IEnumerator doorMove()
    {
        yield return new WaitForSeconds(delayTime); //엘리베이터가 도착하고 delayTime 만큼 지난 이후에 문이 올라가기 시작 
        rigid.velocity = new Vector2(0, doorSpeed);

        yield return new WaitForSeconds(1.1f);    
        isColorBrighten = true; //엘리베이터 문 올라가고 1.1초 후 색이 밝아지기 시작함
        
    }
}
