using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class savePointManager : MonoBehaviour
{
    public int stageNum; //�ش� ���������� �ѹ� 
    public int savePointCount; //�ش� ���� �����ϴ� ���̺�����Ʈ�� ��
    public GameObject[] savePointGroup;
    GameObject player;
    public Vector2 sceneStartPos; //���� � ���̺�����Ʈ�� Ȱ��ȭ���� �ʾ��� �� �����Ǵ� ��ġ 
    public Vector2 sceneStartDir; 

    Vector2 playerSpawnPos; //�÷��̾ �����Ǿ�� �ϴ� ��ġ 
    int playerSpawnSavePoint; //�÷��̾ �����Ǿ�� �ϴ� ���̺�����Ʈ ��ġ: 1,2,3 ~ ���� �ö� 

    private void Awake()
    {
        player = GameObject.Find("Player");
        playerSpawnSavePoint = GameManager.instance.gameData.curAchievementNum;
        //0���� �����ϴ� ���� 
       
        if(playerSpawnSavePoint == 0)
        {
            GameManager.instance.nextPos = sceneStartPos;
            GameManager.instance.nextGravityDir = sceneStartDir;

            GameManager.instance.gameData.respawnScene = SceneManager.GetActiveScene().buildIndex; //���� ������ �ٽ� ��Ȱ�ؾ� ��
            GameManager.instance.gameData.respawnPos = sceneStartPos;
            GameManager.instance.gameData.respawnGravityDir = sceneStartDir;
        }
        else
        {
            //�����̵��� ������ ������Ʈ�� ����: ���̺�����Ʈ, ���� ~> �� �� ������ achievementNum�� ������ ���� 
            for (int index = 0; index < savePointCount; index++)
            {
                if(savePointGroup[index].GetComponent<SavePoint>() != null) //���� ������Ʈ�� ���̺�����Ʈ�̸� 
                {
                    if (savePointGroup[index].GetComponent<SavePoint>().achievementNum == playerSpawnSavePoint)
                    {
                        GameManager.instance.nextPos = savePointGroup[index].GetComponent<SavePoint>().respawnPos;
                        GameManager.instance.nextGravityDir = savePointGroup[index].GetComponent<SavePoint>().respawnDir;

                        GameManager.instance.gameData.respawnPos = savePointGroup[index].GetComponent<SavePoint>().respawnPos;
                        GameManager.instance.gameData.respawnGravityDir = savePointGroup[index].GetComponent<SavePoint>().respawnDir;
                    }
                }
                else //���� ������Ʈ�� key �̸�  
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
