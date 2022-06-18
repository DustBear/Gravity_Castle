using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stage01_elevator_button : MonoBehaviour
{
    [SerializeField] float buttonSpeed = 10f;
    [SerializeField] float moveDistance = 0.5f;
    
    Rigidbody2D rigid;
    Vector2 startPos;
    Vector2 finishPos;
    bool isButtonActivated;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        startPos = transform.localPosition;
        finishPos = startPos + Vector2.down * moveDistance;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player") // player �� ��ư �ݶ��̴� �ǵ帮�� ��ư �����̱� ���� 
        {
            if (isButtonActivated) return; // �̹� ��ư�� Ȱ��ȭ�Ǿ����Ƿ� �÷��̾ �ٽ� �ݶ��̴��� �ǵ���� ��ư�� ������ �־�� ��
            isButtonActivated = true;
            StartCoroutine(Move());
        }
    }

    IEnumerator Move()
    {
        while ((Vector2)transform.localPosition != finishPos)
        {
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, finishPos, buttonSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
