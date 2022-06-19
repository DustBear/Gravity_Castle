using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevatorCage : MonoBehaviour
{
    public int purposePoint; //���������Ͱ� ���� ��ǥ�� �ϴ� ������ ��ȣ
    public bool isAchieved; //���������� ��ǥ ������ �����ߴ����� ���� 
    //1�̸� Pos1, 2�̸� Pos2

    [SerializeField] Vector2 pos1;
    [SerializeField] Vector2 pos2;
    //����: �׻� ���� ��ġ �������� ���� ���� pos1���� �ؾ� �� 
    [SerializeField] float elevatorSpeed;
    [SerializeField] float achieveNumNeeded; //���������� Ȱ��ȭ�� ���� �ʿ��� ��ô��(�� ���� '�̻�'�� �� ���������� �̿밡��)
    [SerializeField] int initialPos; //������ �� ��ġ�� ���� ~> 1�̸� Pos1, 2�̸� Pos2 �� �ȴ�

    [SerializeField] bool isPlayerOnBell; //�÷��̾ �� ���� ���� �ִ����� ���� 
    [SerializeField] float playerAddforce; 
    //���������Ͱ� �Ʒ��� ������ �� ������ϱ� ������ �÷��̾ ���߿� �� �߰� �� ~> ������ ���� ���������� �༭ �ٴڿ� �پ��ְ� ���� 

    Rigidbody2D rigid;
    public GameObject weight; //������ ~> �� �� ��ǥ �߽ɿ� ���� ���������Ϳ� �ݴ�� �������� ��
    public GameObject bell; //���� �︱ ������ ���� �¿�� ������ ��
    public GameObject gear;
    public Animator gearAni;

    Quaternion initialBellRotation; //���� �� ���� ȸ����: ���������Ͱ� ������ �����ִ� ��쿡�� ���� ���������ǿ� ���� ȸ���ؾ� �� 

    private void Awake()
    {
        //�����ϸ鼭 ���������� ��ġ �ʱ�ȭ
        if (initialPos == 1)
        {
            transform.position = pos1;
            purposePoint = 1;
        }
        else
        {
            transform.position = pos2;
            purposePoint = 2;
        }

        isAchieved = true; 
        //�� ó�� ������ ���� purposPoint�� ���� ��ġ�� �����ϵ��� ���� ��� �� 
    }

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        gearAni = gear.GetComponent<Animator>();
        initialBellRotation = bell.transform.rotation;

        gearAni.SetBool("gearMove", false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && collision.transform.rotation == transform.rotation)
        {
            //�÷��̾�� ������������ ȸ������ �ٸ��� Ȱ��ȭ �ȵ�
            InputManager.instance.isJumpBlocked = true; //���������� �������� ���� �Ұ��� 
            isPlayerOnBell = true;
        }       
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            InputManager.instance.isJumpBlocked = false;
            isPlayerOnBell = false;
        }
    }
    void Update()
    {
        if(GameManager.instance.gameData.curAchievementNum < achieveNumNeeded)
        {
            return; 
            //���� ���������͸� �̿��ϱ⿡ ��ô���� ������� ������ ��� ���� 
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isPlayerOnBell) bellRing();
            //���� ���� �÷��̾ �ִ� ���·� ��ȣ�ۿ�Ű ������ �� �︮�鼭 purposePoint �ٲ� 
        }
        
        elevatorMove();
        weightMove();
    }

    void elevatorMove()
    {
        if (isAchieved) return; //��ǥ ������ �����ߴٸ� ���̻� ������ �ʿ� ����

        Vector2 dirVector;
        Vector2 moveDirection;

        if (purposePoint == 1) //���� pos2 ���� pos1���� �̵��ϰ� �ִ� ���̸� 
        {
            dirVector = pos1 - pos2;
            moveDirection = dirVector.normalized; //moveDirection = �������� �ϴ� ����(ũ��1 ���ͷ� ǥ��)
            rigid.velocity = moveDirection * elevatorSpeed; //���������� �ӵ� �Ҵ�    

            if (vectorJudge())
            {
                gearAni.SetBool("gearMove", false);
                transform.position = pos1; //��ġ �����ϰ� 
                rigid.velocity = Vector3.zero; //�ӵ� ������Ű�� 
                isAchieved = true;
            }
        }
        else //���� pos1 ���� pos2 ���� �̵��ϰ� �ִ� ���̸� 
        {
            dirVector = pos2 - pos1;
            moveDirection = dirVector.normalized;
            rigid.velocity = moveDirection * elevatorSpeed; //���������� �ӵ� �Ҵ�    

            if (vectorJudge())
            {
                gearAni.SetBool("gearMove", false);
                transform.position = pos2; //��ġ �����ϰ� 
                rigid.velocity = Vector3.zero; //�ӵ� ������Ű�� 
                isAchieved = true;
            }
        }
               
       
    }
  
    void bellRing() 
    {
        isAchieved = false;

        //���� �︮�� ������ ������ ���� �ٽ� ����� �� �����̼��� ��ø���� �ʵ��� �� 
        StopCoroutine("bellShake");
        bell.transform.rotation = initialBellRotation;
        gearAni.SetBool("gearMove", true); //��� ������

        StartCoroutine("bellShake");

        if (purposePoint == 1)
        {
            purposePoint = 2;
            Vector3 addForceDir = GameObject.Find("Player").transform.TransformDirection(transform.up) * (-1); //�÷��̾�.up �� �ݴ� ������ ����Ų�� 
            //�÷��̾ ƨ���������� �ʰ� ���� ������� �ϴ� ����/�÷��̾��� ȸ������ ���� �޶����� �Ѵ�

            GameObject.Find("Player").GetComponent<Rigidbody2D>().AddForce(addForceDir * playerAddforce, ForceMode2D.Impulse);
            //pos1�� �̵��ϴ� ���� pos2�� ��ǥ �ٲ� ~> �÷��̾� ���߿� ���� �ʰ� ������ �� 

            gearAni.SetFloat("gearSpeed", 2);
        }
        else
        {
            purposePoint = 1;
            gearAni.SetFloat("gearSpeed", -2); //�̵������� �ݴ�� ����� ȸ�����⵵ �ݴ뿩�� �� 
        }
        //���������Ϳ� �޷� �ִ� ���� ���� ~> ���� purposePoint�� 1�̸� 2��, 2�̸� 1�� �ٲ��� 
    }

    IEnumerator bellShake() //���� �¿�� ��鸮�� �ִϸ��̼�
    {
        float maxRotate=30;
        float middleRotate=20;
        float minRotate=10;

        float[] bellRotation = new float[3] { maxRotate, middleRotate, minRotate };

        float curRotate = 0;
        for(int a=0; a<=2; a++)
        {
            curRotate = bellRotation[a];
            for(int b=0; b<=3; b++)
            {
                switch (b)
                {
                    case 0:
                        bell.transform.Rotate(0, 0, curRotate);
                        break;
                    case 1:
                        bell.transform.Rotate(0, 0, -curRotate);
                        break;
                    case 2:
                        bell.transform.Rotate(0, 0, -curRotate);
                        break;
                    case 3:
                        bell.transform.Rotate(0, 0, curRotate);
                        break;
                }
                yield return new WaitForSeconds(0.05f);
            }
        }       
    }

    bool vectorJudge() //���������Ͱ� ���� ���� �ִ��� �Ǵ���
    {
        Vector2 pos1Vector = pos1 - new Vector2(transform.position.x, transform.position.y);
        Vector2 pos2Vector = pos2 - new Vector2(transform.position.x, transform.position.y);

        if(Vector2.Dot(pos1Vector, pos2Vector) > 0)
        {
            //���������ͷκ��� �� �شܱ����� ���͸� ���� ���� �� �����ؼ� ����� �۵����� ��� �� ~> �����ߴ�!
            
            return true; 
        }
        else
        {
            return false;
        }
    }

    void weightMove()
    {
        Vector2 centerPos;
        Vector2 cagePos = new Vector2(transform.position.x, transform.position.y);

        centerPos = (pos1 + pos2) / 2;

        weight.transform.position = 2*centerPos - cagePos;
        //�����ߴ� �׻� ���������� ��� ���� �߽ɿ��� ��Ī�Ǵ� ��ġ�� ���� 
    }
}
