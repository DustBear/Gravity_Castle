using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class savePointManager : MonoBehaviour
{
    GameObject player;
    
    public int stageNum; //현재의 stage 번호 
    public int savePointCount; //현재 씬의 세이브포인트 개수 
    public GameObject[] savePointGroup;
       
    int playerSpawnSavePoint; //0이면 새로 스테이지를 시작한 상황 , 1이상 정수면 해당 세이브에서 스폰해줘야 함 

    private void Awake()
    {
        player = GameObject.Find("Player");
        playerSpawnSavePoint = GameManager.instance.gameData.curAchievementNum; //현재의 achNum 이 스폰돼야 하는 세이브포인트 번호 
      
        if (GameManager.instance.gameData.SpawnSavePoint_bool) //ChangeScene을 통해 이동하거나 씬을 맨 처음 시작할 때는 세이브포인트에서 시작할 필요 x 
        {
            for (int index = 0; index < savePointCount; index++)
            {
                if (savePointGroup[index].GetComponent<SavePoint>().achievementNum == playerSpawnSavePoint)
                {
                    //GameManager 데이터 갱신
                    GameManager.instance.nextPos = savePointGroup[index].GetComponent<SavePoint>().respawnPos;
                    GameManager.instance.nextGravityDir = savePointGroup[index].GetComponent<SavePoint>().respawnDir;

                    //GameData 데이터 갱신
                    GameManager.instance.gameData.respawnPos = savePointGroup[index].GetComponent<SavePoint>().respawnPos;
                    GameManager.instance.gameData.respawnGravityDir = savePointGroup[index].GetComponent<SavePoint>().respawnDir;

                    //GameData 데이터 저장 
                    string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
                    string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[0];
                    File.WriteAllText(filePath, ToJsonData);
                }               
            }
            //세이브포인트에서 소환하고 나면 changeScene 등 이용할 수 있도록 false 처리 해줘야 함 
            GameManager.instance.gameData.SpawnSavePoint_bool = false;
        }
    }
}
