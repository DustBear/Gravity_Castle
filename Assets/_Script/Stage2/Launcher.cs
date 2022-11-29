using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField] ObjManager.ObjType type;
    public Sprite[] spriteGroup;
    //[0]이 장전 안 된 상태 
    //[0] 에서 숫자가 올라가면 장전 중 
    //다시 [0]으로 돌아오면 발사 

    [SerializeField] float launchOffset; //게임 시작 후 최초 발사까지 걸리는 시간
    [SerializeField] float launchPeriod; //투사체 발사 주기

    [SerializeField] float fireSpeed; //발사 시 투사체의 속도  

    [SerializeField] float limitSpeed_arrow; //화살의 최대속도 

    [SerializeField] float initTime;
    //씬이 시작했을 때의 시간 ~> 절대적인 시간을 기준점으로 삼아서 연산오차 누적을 막는다 
    float curTime;
    int launchIndex = 0; //씬 시작 이후 발사한 횟수 

    SpriteRenderer spr;

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        initTime = Time.time;
    }

    private void FixedUpdate()
    {
        LaunchManager();
    }
   
    void LaunchManager()
    {      
        curTime = Time.time; //현재 시각
        if (curTime - initTime >= launchIndex * launchPeriod + launchOffset)
        {
            StartCoroutine(launchAnim());

            launchIndex++;
        }
    }

    void launch()
    {
        GameObject curObj = ObjManager.instance.GetObj(type); //필요한 타입의 발사체 가져옴 
        curObj.transform.position = transform.position + transform.up*0.5f; //위치는 발사기의 위치로부터 발사방향으로 0.5만큼 떨어진 점에 고정 
        curObj.transform.eulerAngles = transform.eulerAngles; //발사각도 고정 

        if (type == ObjManager.ObjType.arrow)
        {
            curObj.GetComponent<stage2_arrow>().limitSpeed = limitSpeed_arrow;
        }

        Rigidbody2D rigid = curObj.GetComponent<Rigidbody2D>();

        curObj.SetActive(true);
        rigid.velocity = transform.up * fireSpeed; //발사기의 위측 방향이 발사 방향 
    }

    IEnumerator launchAnim() //발사 동작은 0.15초동안 이루어짐. 발사주기는 0.15초보다 짧으면 안됨 
    {
        for(int index=1; index<spriteGroup.Length; index++)
        {
            spr.sprite = spriteGroup[index];
            yield return new WaitForSeconds(0.05f);
        }

        launch(); //투사체 발사 
        spr.sprite = spriteGroup[0];
    }
}
