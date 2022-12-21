using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stage2_loopStone_sensor : MonoBehaviour
{
    Player playerScr;

    //stone�� Ÿ�Կ� ���� ���� �ٸ� ������ ��ũ��Ʈ ���� 
    stage2_loopStone stone;
    sensorRes_hammer res_stone;
    int stoneType;

    private void Awake()
    {
        GameObject tmpStone = transform.parent.gameObject;

        if(tmpStone.GetComponent<stage2_loopStone>() != null)
        {
            stone = tmpStone.GetComponent<stage2_loopStone>();
            stoneType = 1;
        }
        else if(tmpStone.GetComponent<sensorRes_hammer>() != null)
        {
            res_stone = tmpStone.GetComponent<sensorRes_hammer>();
            stoneType = 2;
        }
        
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && collision.transform.up == transform.up)
        {
            playerScr = collision.gameObject.GetComponent<Player>();
            //��ġ�� �����̴� ���� �÷��̾ �ε����� ���� + ���� ����ִ� ���·� �ε����� ���� 
            //��ġ�� �÷��̾��� �߷¹����� ���� ���� �׾�� ��. ��ġ�� �������� �����̴� ��Ȳ���� �÷��̾ �� ���� �� �ִ� ���� ���� 

            if (stoneType == 1 && stone.isHitting)
            {
                playerScr.StartCoroutine(playerScr.Die());
            }
            else if(stoneType == 2 && res_stone.isHitting)
            {
                playerScr.StartCoroutine(playerScr.Die());
            }
        }
    }
}
