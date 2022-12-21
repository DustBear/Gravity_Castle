using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sensorRes_hammer : MonoBehaviour
{
    [SerializeField] float hitDelay; //망치가 떨어지기 시작해서 끝에 도달하는 데 걸리는 시간 
    [SerializeField] float backDelay; //망치가 다시 원래 위치로 돌아가는 데 걸리는 시간 

    [SerializeField] Vector2 startPos; //망치의 원래 시작지점
    [SerializeField] Vector2 finishPos; //망치의 도달지점 
    [SerializeField] Vector2 crushPos; //중간에 박스와 부딪힌다고 가정할 때 정지할 위치 

    Rigidbody2D rigid;
    public bool isHitting;

    bool isCorWork;
    float corTimer;

    public bool shouldHit;
    [SerializeField] bool hammerMeetPlatform = false;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        shouldHit = false;
    }

    void Update()
    {
        if(shouldHit && !isCorWork) //물체가 센서 위에 있을 때 계속 망치를 때림 
        {
            StartCoroutine(stone_hit());
        }
    }    
    IEnumerator stone_hit() //망치가 지면을 한 번 때리는 동작 
    {
        isCorWork = true;
        isHitting = true;

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

            if (hammerMeetPlatform) //해머가 내려오는 도중 플랫폼에 부딪히면 멈춰야 함 
            {
                transform.position = crushPos;
                break;
            }
        }

        isHitting = false;
        rigid.velocity = Vector2.zero;

        if (!hammerMeetPlatform)
        {
            transform.position = finishPos;
        }

        //지면을 때린 직후 1초간 멈춤 
        yield return new WaitForSeconds(1f);

        //등속운동하면서 원래 위치로 돌아감 
        rigid.velocity = -dir * (distance / backDelay);
        while(true)
        {
            if(Mathf.Abs(new Vector2(transform.position.x - startPos.x, transform.position.y - startPos.y).magnitude) <= 0.1f)
            {
                rigid.velocity = Vector2.zero; //속도 초기화 
                transform.position = startPos;
                break;
            }

            yield return null;
        }
        
        isCorWork = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Platform") 
        {
            hammerMeetPlatform = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Platform") 
        {
            hammerMeetPlatform = false;
        }
    }
}
