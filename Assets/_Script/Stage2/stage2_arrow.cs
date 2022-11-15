using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class stage2_arrow : MonoBehaviour
{
    ObjManager.ObjType type = ObjManager.ObjType.arrow;
    Scene originScene;

    Rigidbody2D rigid;
    public float limitSpeed; //이 화살의 한계 속도 

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        
    }

    private void OnEnable()
    {
        originScene = SceneManager.GetActiveScene(); //포탄이 생성될 때 마다 씬 번호 가져와서 할당 
    }


    void Update()
    {
        // 씬 번호가 바뀌면 오브젝트 파괴해 줘야 함 
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
            //속도가 일정 수치에 도달하면 더 이상 커지지 않음 
        }
    }
}
