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
        if (collision.gameObject.tag == "Player") // player 가 버튼 콜라이더 건드리면 버튼 움직이기 시작 
        {
            if (isButtonActivated) return; // 이미 버튼이 활성화되었으므로 플레이어가 다시 콜라이더를 건드려도 버튼은 가만히 있어야 함
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
