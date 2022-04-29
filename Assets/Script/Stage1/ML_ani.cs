using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ML_ani : MonoBehaviour
{
    Animator ani;
    GameObject player;
    bool isVibrate;
    void Start()
    {
        ani = gameObject.GetComponent<Animator>();
        ani.SetBool("isPlayerOn", false); //게임 시작하면 false 로 설정 
        player = GameObject.Find("Player");
    }

    private void Update()
    {

    }
    private void OnTriggerStay2D(Collider2D collision) //플레이어가 닿아 있는 상황에 지속적으로 호출 
    {
        if (collision.gameObject.tag == "Player")
        {
            if (player.GetComponent<Rigidbody2D>().velocity.magnitude > 0.1f) //플레이어의 속도가 0.1f 이상일 때 
            {
                ani.SetBool("isPlayerOn", true); //플레이어가 랜턴 내에 있으면서 움직이면 랜턴이 흔들림
            }
            else
            {
                ani.SetBool("isPlayerOn", false); //닿은 물체가 플레이어가 아니라면 false 지정 
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            ani.SetBool("isPlayerOn", false);
        }
    }
}
