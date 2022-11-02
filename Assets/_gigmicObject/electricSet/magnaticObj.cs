using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class magnaticObj : MonoBehaviour
{
    Rigidbody2D rigid;

    [SerializeField] bool isContact; //현재 매그네틱필드에 닿아있는지의 여부 
    [SerializeField] bool isMagnetic; //현재 매그네틱필드에 자기장이 걸려 있는지의 여부

    [SerializeField] bool isPlayerContact; //현재 플레이어와 닿아있는지의 여부 
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
     
    private void OnCollisionEnter2D(Collision2D collision)
    {        
        if (collision.gameObject.tag == "electricPlatform")
        {
            isContact = true;
            magneticField = collision.gameObject;
            transform.parent = magneticField.transform; //이 오브젝트를 매그네틱필드의 자식오브젝트로 넣음 

            initLocalPos = transform.localPosition;

            StartCoroutine(magnetic_check()); 
        }
        else if(collision.gameObject.tag == "Player" && !isMagnetic) //자석에 닿아있는 동안에는 어차피 움직일 수 없으므로 고려 x 
        {
            isPlayerContact = true;

            initPos = transform.position;
            StartCoroutine(playerContact());
        }        
    }

    [SerializeField] Vector3 initPos;

    IEnumerator playerContact()
    {
        initPos = transform.position;
        //플레이어와 닿아있는 동안 플레이어가 뚫고 지나가지 못하게 막음 
        while (isPlayerContact)
        {
            transform.position = initPos;
            yield return null;
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
        else if (collision.gameObject.tag == "Player" && !isMagnetic) //자석에 닿아있는 동안에는 어차피 움직일 수 없으므로 고려 x 
        {
            isPlayerContact = false;
            transform.position = initPos;
        }
    }
   
    private void OnCollisionStay2D(Collision2D collision)
    //자석에 붙어있는 상태에서 전류 끊으면 떨어져야 함 
    //자석에 붙어있는 동안 매 프레임 호출
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
        while(magneticField != null)
        {
            isMagnetic = magneticField.GetComponent<electricSensor>().magWork;
            yield return null;
        }
    }
}
