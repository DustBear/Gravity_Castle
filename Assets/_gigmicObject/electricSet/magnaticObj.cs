using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class magnaticObj : MonoBehaviour
{
    Rigidbody2D rigid;

    [SerializeField] bool isContact; //���� �ű׳�ƽ�ʵ忡 ����ִ����� ���� 
    [SerializeField] bool isMagnetic; //���� �ű׳�ƽ�ʵ忡 �ڱ����� �ɷ� �ִ����� ����

    GameObject magneticField;

    private void Awake()
    {        
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
       
    }

    Vector3 initLocalPos;

    void Update()
    {
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
                //rigid.constraints = RigidbodyConstraints2D.FreezeAll;
                transform.localPosition = initLocalPos;
                rigid.gravityScale = 0f;
            }
            else
            {
                rigid.bodyType = RigidbodyType2D.Dynamic;
                rigid.gravityScale = 3f;
            }
        }
    }
     
    private void OnCollisionEnter2D(Collision2D collision)
    {        
        if (collision.gameObject.tag == "electricPlatform")
        {
            isContact = true;
            magneticField = collision.gameObject;
            transform.parent = magneticField.transform; //�� ������Ʈ�� �ű׳�ƽ�ʵ��� �ڽĿ�����Ʈ�� ���� 

            initLocalPos = transform.localPosition;

            StartCoroutine(magnetic_check()); 
        }
        else if(collision.gameObject.tag == "Player" && !isMagnetic) //�ڼ��� ����ִ� ���ȿ��� ������ ������ �� �����Ƿ� ��� x 
        {
            rigid.velocity = Vector3.zero; //�ӵ� ���� 
        }        
    }

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
   
    private void OnCollisionStay2D(Collision2D collision)
    //�ڼ��� �پ��ִ� ���¿��� ���� ������ �������� �� 
    //�ڼ��� �پ��ִ� ���� �� ������ ȣ��
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
        while(magneticField != null)
        {
            isMagnetic = magneticField.GetComponent<electricSensor>().magWork;
            yield return null;
        }
    }
}
