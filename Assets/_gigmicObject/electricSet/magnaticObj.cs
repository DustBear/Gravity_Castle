using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class magnaticObj : MonoBehaviour
{
    Rigidbody2D rigid;

    [SerializeField] bool isContact; //현재 매그네틱필드에 닿아있는지의 여부 
    [SerializeField] bool isMagnetic; //현재 매그네틱필드에 자기장이 걸려 있는지의 여부

    GameObject magneticField;

    public int activeAchNum; //현재 퍼즐을 풀고 있는 동안만 작동 
    private void Awake()
    {        
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
       if(GameManager.instance.gameData.curAchievementNum != activeAchNum)
        {
            rigid.bodyType = RigidbodyType2D.Kinematic; //비활성화 상태에선 멈춘다 
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
            if (isMagnetic) //매그네틱필드에 닿아있지 않으면 자기장도 없음 
            {
                isMagnetic = false;
            }
            return;
        }
        else
        {
            if (isMagnetic) //자석에 닿아있으면서 전류가 흐르는 상태면 그대로 고정 
            {
                transform.localPosition = initLocalPos; //위치 고정 
                rigid.gravityScale = 0f; //중력 영향 없앰
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
            transform.parent = magneticField.transform; //이 오브젝트를 매그네틱필드의 자식오브젝트로 넣음 
           
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
            transform.parent = magneticField.transform; //이 오브젝트를 매그네틱필드의 자식오브젝트로 넣음 

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
    //자석에 붙어있는 상태에서 전류 끊으면 떨어져야 함 
    //자석에 붙어있는 동안 매 프레임 호출
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
    //자석에 붙어있는 상태에서 전류 끊으면 떨어져야 함 
    //자석에 붙어있는 동안 매 프레임 호출
    {
        while(magneticField != null) //현재 붙어있는 매그네틱필드의 magWork 값 매 프레임 받아옴 
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
