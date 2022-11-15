using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField] ObjManager.ObjType type;

    [SerializeField] float launchOffset; //게임 시작 후 최초 발사까지 걸리는 시간
    [SerializeField] float launchPeriod; //투사체 발사 주기

    [SerializeField] float fireSpeed; //발사 시 투사체의 속도  

    [SerializeField] float limitSpeed_arrow; //화살의 최대속도 
    void Start()
    {
        StartCoroutine(Launch());
    }

    IEnumerator Launch()
    {
        yield return new WaitForSeconds(launchOffset);

        while (true)
        {            
            GameObject curObj = ObjManager.instance.GetObj(type); //필요한 타입의 발사체 가져옴 
            curObj.transform.position = transform.position + transform.up; //위치는 발사기의 위치로부터 발사방향으로 1만큼 떨어진 점에 고정 
            curObj.transform.eulerAngles = transform.eulerAngles; //발사각도 고정 

            if(type == ObjManager.ObjType.arrow)
            {
                curObj.GetComponent<stage2_arrow>().limitSpeed = limitSpeed_arrow;
            }

            Rigidbody2D rigid = curObj.GetComponent<Rigidbody2D>();
            
            curObj.SetActive(true);
            rigid.velocity = transform.up * fireSpeed; //발사기의 위측 방향이 발사 방향 
            yield return new WaitForSeconds(launchPeriod);
        }
    }

}
