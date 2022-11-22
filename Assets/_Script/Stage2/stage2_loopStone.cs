using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stage2_loopStone : MonoBehaviour
{
    [SerializeField] float initOffset; //씬이 시작하고 맨 처음 망치가 떨어지는 데 걸리는 시간 
    [SerializeField] float hitDelay; //망치가 떨어지기 시작해서 끝에 도달하는 데 걸리는 시간 
    [SerializeField] float backDelay; //망치가 다시 원래 위치로 돌아가는 데 걸리는 시간 
    [SerializeField] float hitPeriod; //망치가 한 번 떨어지고 다음 번 떨어질 때 까지 걸리는 시간 

    [SerializeField] Vector2 startPos; //망치의 원래 시작지점
    [SerializeField] Vector2 finishPos; //망치의 도달지점 

    float timer;
    float corTimer; //코루틴 내부에서 사용하는 타이머 
    bool isCorWork;
    bool isFirstShot = true;

    Rigidbody2D rigid;

    bool isPlayerOn = false;
    GameObject playerObj;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        playerObj = GameObject.Find("Player");
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hitManager();
        playerDieManager();
    }

    void hitManager()
    {
        if (!isCorWork) //코루틴이 작동하지 않을 때만 타이머가 흘러감 
        {
            timer += Time.deltaTime;
        }

        if(isFirstShot && timer >= initOffset)
        {
            StartCoroutine(stone_hit());
            isFirstShot = false;
            timer = 0f;

            return;
        }

        if(!isFirstShot && timer >= hitPeriod)
        {
            StartCoroutine(stone_hit());
            timer = 0f;
        }
    }

    IEnumerator stone_hit() //망치가 지면을 한 번 때리는 동작 
    {
        isCorWork = true;
        float distance = (finishPos - startPos).magnitude;
        Vector2 dir = (finishPos - startPos).normalized;
        float maxSpeed = 2 * distance / hitDelay;

        corTimer = 0f;

        //가속운동하면서 지면을 때림 
        while (corTimer <= hitDelay)
        {
            rigid.velocity = maxSpeed * (corTimer / hitDelay) * dir;

            corTimer += Time.deltaTime;
            yield return null;
        }

        rigid.velocity = Vector2.zero;
        transform.position = finishPos;

        //지면을 때린 직후 0.5초간 멈춤 
        yield return new WaitForSeconds(0.5f);

        //등속운동하면서 원래 위치로 돌아감 
        rigid.velocity = -dir * (distance / backDelay);
        yield return new WaitForSeconds(backDelay);

        rigid.velocity = Vector2.zero; //속도 초기화 
        transform.position = startPos;

        isCorWork = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            isPlayerOn = true;
        }
    }

    void playerDieManager()
    {
        if(isPlayerOn && rigid.velocity.magnitude >= 0.1f && playerObj.GetComponent<Player>().isGrounded)
        {
            //현재 플레이어가 망치에 닿아 있고 + 망치의 속도가 양수이며 + 플레이어가 땅에도 닿아있는 상태이면 
            playerObj.GetComponent<Player>().Die(); //플레이어 사망
        }
    }
}
