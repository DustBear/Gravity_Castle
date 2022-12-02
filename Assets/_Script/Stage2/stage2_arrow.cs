using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class stage2_arrow : MonoBehaviour
{
    ObjManager.ObjType type = ObjManager.ObjType.arrow;
    Scene originScene;

    Rigidbody2D rigid;
    public float limitSpeed; //�� ȭ���� �Ѱ� �ӵ� 

    SpriteRenderer spr;
    bool isArrowHit = false; //ȭ���� ���� �¾Ҵ����� ���� 
    Vector3 lastRot; //���� �������� ȭ�� ���� 
    Vector3 lastPos; //���� �������� ȭ�� ��ġ 
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        
    }

    private void OnEnable()
    {
        isArrowHit = false;
        GetComponent<BoxCollider2D>().enabled = true;
        rigid.gravityScale = 3f;
        spr.color = new Color(1, 1, 1, 1);

        originScene = SceneManager.GetActiveScene(); //��ź�� ������ �� ���� �� ��ȣ �����ͼ� �Ҵ� 
    }


    void Update()
    {
        // �� ��ȣ�� �ٲ�� ������Ʈ �ı��� ��� �� 
        if (originScene != SceneManager.GetActiveScene() || GameManager.instance.gameData.SpawnSavePoint_bool)
        {
            ObjManager.instance.ReturnObj(type, gameObject);
        }

        if (!isArrowHit)
        {
            //ȭ���� ���� ���� ���¿����� �ӵ��� 0�̹Ƿ� �ӵ��� ���� ��� x 
            float angle = Mathf.Atan2(rigid.velocity.y, rigid.velocity.x);
            transform.localEulerAngles = new Vector3(0, 0, 90f + angle * 180f / Mathf.PI);

            lastRot = transform.localEulerAngles;
            lastPos = transform.position;
        }
        else //ȭ���� ���� ���� ��Ȳ 
        {
            transform.localEulerAngles = lastRot;
            transform.position = lastPos;
        }

        limitSpeedCheck();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Platform")
        {
            StartCoroutine(arrowDestroy());
        }
    }

    void limitSpeedCheck()
    {
        if(rigid.velocity.magnitude > limitSpeed)
        {
            rigid.velocity = rigid.velocity * (limitSpeed / rigid.velocity.magnitude);
            //�ӵ��� ���� ��ġ�� �����ϸ� �� �̻� Ŀ���� ���� 
        }
    }

    IEnumerator arrowDestroy() //���ư��� ȭ���� ���� �ε����� �״�� ���� �� ������ ����� 
    {
        isArrowHit = true;
        rigid.velocity = Vector2.zero;

        GetComponent<BoxCollider2D>().enabled = false; //���� ���� ȭ�쿡 �÷��̾ ��� ���� �ʵ��� �ݶ��̴� ����
        
        float arrowFadeTime = 1f;
        float fadeTimer = 0f;

        while(spr.color.a >= 0)
        {
            spr.color = new Color(1, 1, 1, 1 - fadeTimer / arrowFadeTime);
            fadeTimer += Time.deltaTime;

            yield return null;
        }
        ObjManager.instance.ReturnObj(type, gameObject);

    }
}
