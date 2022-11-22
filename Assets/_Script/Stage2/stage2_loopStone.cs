using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stage2_loopStone : MonoBehaviour
{
    [SerializeField] float initOffset; //���� �����ϰ� �� ó�� ��ġ�� �������� �� �ɸ��� �ð� 
    [SerializeField] float hitDelay; //��ġ�� �������� �����ؼ� ���� �����ϴ� �� �ɸ��� �ð� 
    [SerializeField] float backDelay; //��ġ�� �ٽ� ���� ��ġ�� ���ư��� �� �ɸ��� �ð� 
    [SerializeField] float hitPeriod; //��ġ�� �� �� �������� ���� �� ������ �� ���� �ɸ��� �ð� 

    [SerializeField] Vector2 startPos; //��ġ�� ���� ��������
    [SerializeField] Vector2 finishPos; //��ġ�� �������� 

    float timer;
    float corTimer; //�ڷ�ƾ ���ο��� ����ϴ� Ÿ�̸� 
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
        if (!isCorWork) //�ڷ�ƾ�� �۵����� ���� ���� Ÿ�̸Ӱ� �귯�� 
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

    IEnumerator stone_hit() //��ġ�� ������ �� �� ������ ���� 
    {
        isCorWork = true;
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
        }

        rigid.velocity = Vector2.zero;
        transform.position = finishPos;

        //������ ���� ���� 0.5�ʰ� ���� 
        yield return new WaitForSeconds(0.5f);

        //��ӿ�ϸ鼭 ���� ��ġ�� ���ư� 
        rigid.velocity = -dir * (distance / backDelay);
        yield return new WaitForSeconds(backDelay);

        rigid.velocity = Vector2.zero; //�ӵ� �ʱ�ȭ 
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
            //���� �÷��̾ ��ġ�� ��� �ְ� + ��ġ�� �ӵ��� ����̸� + �÷��̾ ������ ����ִ� �����̸� 
            playerObj.GetComponent<Player>().Die(); //�÷��̾� ���
        }
    }
}
