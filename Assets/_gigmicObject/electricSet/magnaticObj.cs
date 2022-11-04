using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class magnaticObj : MonoBehaviour
{
    Rigidbody2D rigid;

    [SerializeField] bool isContact; //���� �ű׳�ƽ�ʵ忡 ����ִ����� ���� 
    [SerializeField] bool isMagnetic; //���� �ű׳�ƽ�ʵ忡 �ڱ����� �ɷ� �ִ����� ����

    GameObject magneticField;

    public int activeAchNum; //���� ������ Ǯ�� �ִ� ���ȸ� �۵� 
    private void Awake()
    {        
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
       if(GameManager.instance.gameData.curAchievementNum != activeAchNum)
        {
            rigid.bodyType = RigidbodyType2D.Kinematic; //��Ȱ��ȭ ���¿��� ����� 
        }
        else
        {
            rigid.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    Vector3 initLocalPos;

    void Update()
    {
        if(GameManager.instance.gameData.curAchievementNum != activeAchNum)
        {
            return;
        }
        else
        {
            if(rigid.bodyType != RigidbodyType2D.Dynamic)
            {
                rigid.bodyType = RigidbodyType2D.Dynamic;
            }
        }

        if (!isContact)
        {
            if (isMagnetic) //�ű׳�ƽ�ʵ忡 ������� ������ �ڱ��嵵 ���� 
            {
                isMagnetic = false;
            }
            return;
        }
        else
        {
            if (isMagnetic) //�ڼ��� ��������鼭 ������ �帣�� ���¸� �״�� ���� 
            {
                transform.localPosition = initLocalPos; //��ġ ���� 
                rigid.gravityScale = 0f; //�߷� ���� ����
            }
            else
            {
                rigid.gravityScale = 3f;
            }
        }
    }
    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {        
        if (collision.gameObject.tag == "electricPlatform")
        {
            isContact = true;
            magneticField = collision.gameObject;
            transform.parent = magneticField.transform; //�� ������Ʈ�� �ű׳�ƽ�ʵ��� �ڽĿ�����Ʈ�� ���� 
           
            StartCoroutine(magnetic_check()); 
        }
    }
    */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "electricPlatform")
        {
            isContact = true;
            magneticField = collision.gameObject;
            transform.parent = magneticField.transform; //�� ������Ʈ�� �ű׳�ƽ�ʵ��� �ڽĿ�����Ʈ�� ���� 

            StartCoroutine(magnetic_check());
        }
    }
    /*
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "electricPlatform")
        {
            magneticField = null;
            transform.parent = null;
            isContact = false;
            isMagnetic = false;
        }
    }
    */
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "electricPlatform")
        {
            magneticField = null;
            transform.parent = null;
            isContact = false;
            isMagnetic = false;
        }
    }
    /*
    private void OnCollisionStay2D(Collision2D collision)
    //�ڼ��� �پ��ִ� ���¿��� ���� ������ �������� �� 
    //�ڼ��� �پ��ִ� ���� �� ������ ȣ��
    {
        if (collision.gameObject.tag == "electricPlatform")
        {
            isMagnetic = collision.gameObject.GetComponent<electricSensor>().magWork;
        }
    }
    */
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "electricPlatform")
        {
            isMagnetic = collision.gameObject.GetComponent<electricSensor>().magWork;
        }
    }

    IEnumerator magnetic_check()
    //�ڼ��� �پ��ִ� ���¿��� ���� ������ �������� �� 
    //�ڼ��� �پ��ִ� ���� �� ������ ȣ��
    {
        while(magneticField != null) //���� �پ��ִ� �ű׳�ƽ�ʵ��� magWork �� �� ������ �޾ƿ� 
        {
            isMagnetic = magneticField.GetComponent<electricSensor>().magWork;
            if (isMagnetic)
            {
                initLocalPos = transform.localPosition;
            }
            yield return null;
        }
    }
}
