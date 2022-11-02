using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectPlayerCrash : MonoBehaviour
{
    //�� ��ũ��Ʈ�� �پ��ִ� ������Ʈ�� ���� �ӵ� �̻����� �����ϴٰ� �÷��̾�� �ε����� �÷��̾ ���� 

    Rigidbody2D rigid;
    public float thresholdSpeed;
    //�� �ӵ� �̻����� �÷��̾�� �浹�ϸ� ��� 

    bool shouldVelCheck = true;

    private void Awake()
    {
        if(GetComponent<Rigidbody2D>() != null)
        {
            //rigidBody�� �����Ѵٸ� ������ 
            rigid = GetComponent<Rigidbody2D>(); 
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(rigid == null)
        {
            return;
        }

        //�� ������Ʈ ���� �ӵ����� ���ؼ� ���� 
        if (shouldVelCheck)
        {
            curVel = rigid.velocity;
        }
    }
    GameObject playerObj;
    Player playerScr;
    Vector2 curVel;

    IEnumerator playerCrash()
    {
        Vector2 movingDir = new Vector2(curVel.x, curVel.y); //�浹�������� ������Ʈ �̵����� 
        Vector2 playerDir = playerObj.transform.position - transform.position; //������Ʈ���� �÷��̾������ �Ÿ� 

        Vector2 toDir = playerDir - movingDir;
        //float degree = Mathf.Atan2(toDir.y, toDir.x) * Mathf.Rad2Deg;
        //
        float degree = Mathf.Acos(Vector2.Dot(movingDir, playerDir) / (movingDir.magnitude * playerDir.magnitude)) * Mathf.Rad2Deg;
        //���� ������Ʈ�� �̵����� ���Ϳ� �÷��̾���� ����� ��ġ���� ������ �� ~> �浹�� �ǹ� 

        Debug.Log("movingDIr: " + movingDir +  ", playerDir: " + playerDir + ", degree: " + degree);

        if(Mathf.Abs(degree) <= 45f && curVel.magnitude >= thresholdSpeed) //����ġ �̻� �ӵ��� �浹�ϸ� �浹���� ������ 45�� ������ �� 
        {
            playerScr = playerObj.GetComponent<Player>();
            yield return playerScr.StartCoroutine(playerScr.Die()); //�÷��̾� �浹�� ���
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        shouldVelCheck = false; 
        //�÷��̾�� �浹�ϸ� �ӵ����� ����
        //���� �浹 ������ �ӵ��� ������ �ǰ� �� 

        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("player Detect, speed: " + curVel.magnitude);

            playerObj = collision.gameObject;
            playerScr = playerObj.GetComponent<Player>();

            if (!playerScr.isDieCorWork)
            {
                StartCoroutine(playerCrash());
            }
        }

        shouldVelCheck = true;
    }
}
