using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class savePointManager : MonoBehaviour
{
    GameObject player;
    
    public int stageNum; //�ش� ���������� �ѹ� 
    public int savePointCount; //�ش� ���� �����ϴ� ���̺�����Ʈ�� ��
    public GameObject[] savePointGroup;
       
    int playerSpawnSavePoint; //�÷��̾ �����Ǿ�� �ϴ� ���̺�����Ʈ ��ġ: 1,2,3 ~ ���� �ö� 

    private void Awake()
    {
        //Debug.Log("savePointManager activated");

        player = GameObject.Find("Player");
        playerSpawnSavePoint = GameManager.instance.gameData.curAchievementNum; //0���� �����ϴ� ���� 
      
        if (GameManager.instance.shouldSpawnSavePoint) //ChangeScene�� �̿��� ���� ���� ���� ���̺�����Ʈ ���x 
        {
            for (int index = 0; index < savePointCount; index++)
            {
                if (savePointGroup[index].GetComponent<SavePoint>().achievementNum == playerSpawnSavePoint)
                {
                    //���̺�����Ʈ���� GM �� ���� 
                    GameManager.instance.nextPos = savePointGroup[index].GetComponent<SavePoint>().respawnPos;
                    GameManager.instance.nextGravityDir = savePointGroup[index].GetComponent<SavePoint>().respawnDir;

                    //���̺�����Ʈ���� GameData �� ���� 
                    GameManager.instance.gameData.respawnPos = savePointGroup[index].GetComponent<SavePoint>().respawnPos;
                    GameManager.instance.gameData.respawnGravityDir = savePointGroup[index].GetComponent<SavePoint>().respawnDir;

                    //GameData �� ���� 
                    string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
                    string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[0];
                    File.WriteAllText(filePath, ToJsonData);
                }               
            }
            GameManager.instance.shouldSpawnSavePoint = false;
        }
    }
}
