using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stage2_loopStone_sensor : MonoBehaviour
{
    Player playerScr;

    //stone의 타입에 따라 각기 다른 변수에 스크립트 저장 
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
            //망치가 움직이는 도중 플레이어가 부딪히면 죽음 + 땅에 닿아있는 상태로 부딪혀야 죽음 
            //망치와 플레이어의 중력방향이 같을 때만 죽어야 함. 망치가 위쪽으로 움직이는 상황에서 플레이어가 그 위에 서 있는 경우는 제외 

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
