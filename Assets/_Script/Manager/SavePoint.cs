using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePoint : MonoBehaviour
{
    [SerializeField] int stageNum;
    [SerializeField] int achievementNum; //1부터 시작

    Transform player;
    Vector2 respawnPos; //플레이어가 세이브포인트에 닿고 나서 리스폰되는 위치

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        respawnPos = transform.position;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && 
            transform.eulerAngles.z == player.eulerAngles.z && 
            GameManager.instance.gameData.curAchievementNum == achievementNum - 1 || GameManager.instance.gameData.curStageNum == stageNum - 1)
        {
            //플레이어가 세이브포인트와 같은 angle을 가지고 있고, 플레이어가 이 세이브포인트의 바로 전 세이브포인트까지 활성화시켰을 때만 작동 
            GameManager.instance.SaveData(achievementNum, stageNum, respawnPos);
        }
    }
}
