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
        ani.SetBool("isPlayerOn", false); //���� �����ϸ� false �� ���� 
        player = GameObject.Find("Player");
    }

    private void Update()
    {

    }
    private void OnTriggerStay2D(Collider2D collision) //�÷��̾ ��� �ִ� ��Ȳ�� ���������� ȣ�� 
    {
        if (collision.gameObject.tag == "Player")
        {
            if (player.GetComponent<Rigidbody2D>().velocity.magnitude > 0.1f) //�÷��̾��� �ӵ��� 0.1f �̻��� �� 
            {
                ani.SetBool("isPlayerOn", true); //�÷��̾ ���� ���� �����鼭 �����̸� ������ ��鸲
            }
            else
            {
                ani.SetBool("isPlayerOn", false); //���� ��ü�� �÷��̾ �ƴ϶�� false ���� 
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
