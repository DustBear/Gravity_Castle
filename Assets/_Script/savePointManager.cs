using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class savePointManager : MonoBehaviour
{
    GameObject player;
    
    public int stageNum; //해당 스테이지의 넘버 
    public int savePointCount; //해당 씬에 존재하는 세이브포인트의 수
    public GameObject[] savePointGroup;
       
    int playerSpawnSavePoint; //플레이어가 스폰되어야 하는 세이브포인트 위치: 1,2,3 ~ 으로 올라감 

    private void Awake()
    {
        player = GameObject.Find("Player");
        playerSpawnSavePoint = GameManager.instance.gameData.curAchievementNum; //0부터 시작하는 정수 

        if (GameManager.instance.shouldSpawnSavePoint)
        {
            for (int index = 0; index < savePointCount; index++)
            {              
                    if (savePointGroup[index].GetComponent<SavePoint>().achievementNum == playerSpawnSavePoint)
                    {
                        GameManager.instance.nextPos = savePointGroup[index].GetComponent<SavePoint>().respawnPos;
                        GameManager.instance.nextGravityDir = savePointGroup[index].GetComponent<SavePoint>().respawnDir;

                        GameManager.instance.gameData.respawnPos = savePointGroup[index].GetComponent<SavePoint>().respawnPos;
                        GameManager.instance.gameData.respawnGravityDir = savePointGroup[index].GetComponent<SavePoint>().respawnDir;
                    }               
            }

            GameManager.instance.shouldSpawnSavePoint = false;
        }

        
       


    }   
}
