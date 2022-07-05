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
        GameData curGameData = GameManager.instance.gameData;
        if (other.CompareTag("Player") && transform.eulerAngles.z == player.eulerAngles.z) //플레이어가 세이브포인트와 같은 angle을 가지고 있을 때
        {
            if (stageNum == curGameData.curStageNum) // 세이브포인트의 stage와 저장된 stage가 같을 때
            {
                if (achievementNum == curGameData.curAchievementNum + 1)
                {
                    SaveData();
                }
            }
            else if (achievementNum == 1)
            {
                SaveData();
            }
        }
    }

    void SaveData()
    {
        Debug.Log("savePointBackUp: " + achievementNum);
        GameManager.instance.SaveData(achievementNum, stageNum, respawnPos);
    }
}
