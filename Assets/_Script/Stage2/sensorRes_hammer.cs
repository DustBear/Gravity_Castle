using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sensorRes_hammer : MonoBehaviour
{
    [SerializeField] float hitDelay; //��ġ�� �������� �����ؼ� ���� �����ϴ� �� �ɸ��� �ð� 
    [SerializeField] float backDelay; //��ġ�� �ٽ� ���� ��ġ�� ���ư��� �� �ɸ��� �ð� 

    [SerializeField] Vector2 startPos; //��ġ�� ���� ��������
    [SerializeField] Vector2 finishPos; //��ġ�� �������� 
    [SerializeField] Vector2 crushPos; //�߰��� �ڽ��� �ε����ٰ� ������ �� ������ ��ġ 

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
        if(shouldHit && !isCorWork) //��ü�� ���� ���� ���� �� ��� ��ġ�� ���� 
        {
            StartCoroutine(stone_hit());
        }
    }    
    IEnumerator stone_hit() //��ġ�� ������ �� �� ������ ���� 
    {
        isCorWork = true;
        isHitting = true;

        float distance = (finishPos - startPos).magnitude;
        Vector2 dir = (finishPos - startPos).normalized;
        float maxSpeed = 2 * distance / hitDelay;

        corTimer = 0f;

        //���ӿ�ϸ鼭 ������ ���� 
        while (corTimer <= hitDelay)
        {
            rigid.velocity = maxSpeed * (corTimer / hitDelay) * dir;

            corTimer += Time.deltaTime;
            yield return null;

            if (hammerMeetPlatform) //�ظӰ� �������� ���� �÷����� �ε����� ����� �� 
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

        //������ ���� ���� 1�ʰ� ���� 
        yield return new WaitForSeconds(1f);

        //��ӿ�ϸ鼭 ���� ��ġ�� ���ư� 
        rigid.velocity = -dir * (distance / backDelay);
        while(true)
        {
            if(Mathf.Abs(new Vector2(transform.position.x - startPos.x, transform.position.y - startPos.y).magnitude) <= 0.1f)
            {
                rigid.velocity = Vector2.zero; //�ӵ� �ʱ�ȭ 
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
