using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stage2_loopStone_sensor : MonoBehaviour
{
    Player playerScr;
    stage2_loopStone stone;

    private void Awake()
    {
        stone = transform.parent.gameObject.GetComponent<stage2_loopStone>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerScr = collision.gameObject.GetComponent<Player>();
            //��ġ�� �����̴� ���� �÷��̾ �ε����� ���� + ���� ����ִ� ���·� �ε����� ���� 

            if (stone.isHitting)
            {
                playerScr.StartCoroutine(playerScr.Die());
            }
        }
    }
}
