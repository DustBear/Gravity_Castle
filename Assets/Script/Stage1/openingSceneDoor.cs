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
    float curTime;
    public bool isDoorMove;
    public Collider2D[] coll = new Collider2D[3]; //문이 열릴 때 콜라이더를 없애줘야 한다. 따라서 문 스크립트에서 제어하는것이 합리적
    Rigidbody2D rigid;
    void Start()
    {
        player.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("openingSceneObject");
        transform.localPosition = new Vector2(0, startYpos);
        isDoorMove = false;
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDoorMove) 
        {
            StartCoroutine(doorMove()); //인자가 들어오면 문을 연다
        }
    
        if (transform.localPosition.y >= finishYpos)
        {
            transform.localPosition = new Vector2(0, finishYpos); //finishYpos에 도착하면 문 정지 
            isDoorMove = false; //isMove 는 false로 설정 

            for (int index = 0; index < 3; index++)
            {
                coll[index].gameObject.SetActive(false); //엘리베이터 도착하면 콜라이더 꺼야 함, 단 바닥은 그대로 둠
            }
        }
    }

    IEnumerator doorMove()
    {
        yield return new WaitForSeconds(delayTime);
        rigid.velocity = new Vector2(0, doorSpeed);

        yield return new WaitForSeconds(1f);
        player.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Player");
    }
}
