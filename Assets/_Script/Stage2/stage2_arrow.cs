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

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        
    }

    private void OnEnable()
    {
        originScene = SceneManager.GetActiveScene(); //��ź�� ������ �� ���� �� ��ȣ �����ͼ� �Ҵ� 
    }


    void Update()
    {
        // �� ��ȣ�� �ٲ�� ������Ʈ �ı��� ��� �� 
        if (originScene != SceneManager.GetActiveScene() || GameManager.instance.shouldSpawnSavePoint)
        {
            ObjManager.instance.ReturnObj(type, gameObject);
        }

        float angle = Mathf.Atan2(rigid.velocity.y, rigid.velocity.x);
        transform.localEulerAngles = new Vector3(0, 0, 90f + angle * 180f / Mathf.PI);

        limitSpeedCheck();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Platform")
        {
            ObjManager.instance.ReturnObj(type, gameObject);
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
}
