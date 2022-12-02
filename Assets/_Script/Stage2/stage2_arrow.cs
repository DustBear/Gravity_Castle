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

    SpriteRenderer spr;
    bool isArrowHit = false; //화살이 벽에 맞았는지의 여부 
    Vector3 lastRot; //이전 프레임의 화살 각도 
    Vector3 lastPos; //이전 프레임의 화살 위치 
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

        originScene = SceneManager.GetActiveScene(); //포탄이 생성될 때 마다 씬 번호 가져와서 할당 
    }


    void Update()
    {
        // 씬 번호가 바뀌면 오브젝트 파괴해 줘야 함 
        if (originScene != SceneManager.GetActiveScene() || GameManager.instance.gameData.SpawnSavePoint_bool)
        {
            ObjManager.instance.ReturnObj(type, gameObject);
        }

        if (!isArrowHit)
        {
            //화살이 벽에 박힌 상태에서는 속도가 0이므로 속도로 각도 계산 x 
            float angle = Mathf.Atan2(rigid.velocity.y, rigid.velocity.x);
            transform.localEulerAngles = new Vector3(0, 0, 90f + angle * 180f / Mathf.PI);

            lastRot = transform.localEulerAngles;
            lastPos = transform.position;
        }
        else //화살이 벽에 박힌 상황 
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
            //속도가 일정 수치에 도달하면 더 이상 커지지 않음 
        }
    }

    IEnumerator arrowDestroy() //날아가던 화살이 벽에 부딪히면 그대로 박힌 뒤 서서히 사라짐 
    {
        isArrowHit = true;
        rigid.velocity = Vector2.zero;

        GetComponent<BoxCollider2D>().enabled = false; //벽에 박힌 화살에 플레이어가 닿아 죽지 않도록 콜라이더 해제
        
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
