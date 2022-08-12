using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class savePointManager : MonoBehaviour
{
    public int stageNum; //해당 스테이지의 넘버 
    public int savePointCount; //해당 씬에 존재하는 세이브포인트의 수
    public GameObject[] savePointGroup;
    GameObject player;
    public Vector2 sceneStartPos; //아직 어떤 세이브포인트도 활성화하지 않았을 때 스폰되는 위치 
    public Vector2 sceneStartDir; 

    Vector2 playerSpawnPos; //플레이어가 스폰되어야 하는 위치 
    int playerSpawnSavePoint; //플레이어가 스폰되어야 하는 세이브포인트 위치: 1,2,3 ~ 으로 올라감 

    private void Awake()
    {
        player = GameObject.Find("Player");
        playerSpawnSavePoint = GameManager.instance.gameData.curAchievementNum;
        //0부터 시작하는 정수 
       
        if(playerSpawnSavePoint == 0)
        {
            GameManager.instance.nextPos = sceneStartPos;
            GameManager.instance.nextGravityDir = sceneStartDir;

            GameManager.instance.gameData.respawnScene = SceneManager.GetActiveScene().buildIndex; //현재 씬에서 다시 부활해야 함
            GameManager.instance.gameData.respawnPos = sceneStartPos;
            GameManager.instance.gameData.respawnGravityDir = sceneStartDir;
        }
        else
        {
            //빠른이동이 가능한 오브젝트의 종류: 세이브포인트, 열쇠 ~> 둘 다 변수에 achievementNum을 가지고 있음 
            for (int index = 0; index < savePointCount; index++)
            {
                if(savePointGroup[index].GetComponent<SavePoint>() != null) //만약 오브젝트가 세이브포인트이면 
                {
                    if (savePointGroup[index].GetComponent<SavePoint>().achievementNum == playerSpawnSavePoint)
                    {
                        GameManager.instance.nextPos = savePointGroup[index].GetComponent<SavePoint>().respawnPos;
                        GameManager.instance.nextGravityDir = savePointGroup[index].GetComponent<SavePoint>().respawnDir;

                        GameManager.instance.gameData.respawnPos = savePointGroup[index].GetComponent<SavePoint>().respawnPos;
                        GameManager.instance.gameData.respawnGravityDir = savePointGroup[index].GetComponent<SavePoint>().respawnDir;
                    }
                }
                else //만약 오브젝트가 key 이면  
                {
                    if (savePointGroup[index].GetComponent<Key>().achievementNum == playerSpawnSavePoint)
                    {
                        GameManager.instance.nextPos = savePointGroup[index].GetComponent<Key>().respawnPos;
                        GameManager.instance.nextGravityDir = savePointGroup[index].GetComponent<Key>().respawnDir;

                        GameManager.instance.gameData.respawnPos = savePointGroup[index].GetComponent<Key>().respawnPos;
                        GameManager.instance.gameData.respawnGravityDir = savePointGroup[index].GetComponent<Key>().respawnDir;
                    }
                }

            }
        }
    }   
}
