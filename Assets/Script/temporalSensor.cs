using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temporalSensor : MonoBehaviour
{
    //플레이어의 interactive Text가 씬을 시작할 때 계속해서 출력되는 버그가 있음 
    //해결하기엔 시간이 부족하므로 일단 플레이어의 시작지점 바로 위에 temporal collider 를 만든 후 
    //이 collider를 통과할 때 interactive Text를 꺼 주는 방식으로 일단 해결했음.
    //차후에 시간을 들여 고칠 것
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
