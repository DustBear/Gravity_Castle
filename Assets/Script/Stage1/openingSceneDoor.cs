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
    public Collider2D[] coll = new Collider2D[3]; //���� ���� �� �ݶ��̴��� ������� �Ѵ�. ���� �� ��ũ��Ʈ���� �����ϴ°��� �ո���
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
            StartCoroutine(doorMove()); //���ڰ� ������ ���� ����
        }
    
        if (transform.localPosition.y >= finishYpos)
        {
            transform.localPosition = new Vector2(0, finishYpos); //finishYpos�� �����ϸ� �� ���� 
            isDoorMove = false; //isMove �� false�� ���� 

            for (int index = 0; index < 3; index++)
            {
                coll[index].gameObject.SetActive(false); //���������� �����ϸ� �ݶ��̴� ���� ��, �� �ٴ��� �״�� ��
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
