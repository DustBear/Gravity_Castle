using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class savePointManager : MonoBehaviour
{
    GameObject player;
    
    public int stageNum; //해당 스테이지의 넘버 
    public int savePointCount; //해당 씬에 존재하는 세이브포인트의 수
    public GameObject[] savePointGroup;
       
    int playerSpawnSavePoint; //플레이어가 스폰되어야 하는 세이브포인트 위치: 1,2,3 ~ 으로 올라감 

    private void Awake()
    {
        Debug.Log("savePointManager activated");

        player = GameObject.Find("Player");
        playerSpawnSavePoint = GameManager.instance.gameData.curAchievementNum; //0부터 시작하는 정수 
      
        if (GameManager.instance.shouldSpawnSavePoint) //ChangeScene을 이용해 씬을 오갈 때는 세이브포인트 사용x 
        {
            for (int index = 0; index < savePointCount; index++)
            {
                if (savePointGroup[index].GetComponent<SavePoint>().achievementNum == playerSpawnSavePoint)
                {
                    //세이브포인트에서 GM 값 조정 
                    GameManager.instance.nextPos = savePointGroup[index].GetComponent<SavePoint>().respawnPos;
                    GameManager.instance.nextGravityDir = savePointGroup[index].GetComponent<SavePoint>().respawnDir;

                    //세이브포인트에서 GameData 값 조정 
                    GameManager.instance.gameData.respawnPos = savePointGroup[index].GetComponent<SavePoint>().respawnPos;
                    GameManager.instance.gameData.respawnGravityDir = savePointGroup[index].GetComponent<SavePoint>().respawnDir;

                    //GameData 값 저장 
                    string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
                    string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[0];
                    File.WriteAllText(filePath, ToJsonData);
                }               
            }
            GameManager.instance.shouldSpawnSavePoint = false;
        }
    }
}
