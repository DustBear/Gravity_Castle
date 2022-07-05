using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePoint : MonoBehaviour
{
    [SerializeField] int stageNum;
    [SerializeField] int achievementNum; //1���� ����

    Transform player;
    Vector2 respawnPos; //�÷��̾ ���̺�����Ʈ�� ��� ���� �������Ǵ� ��ġ

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        respawnPos = transform.position;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        GameData curGameData = GameManager.instance.gameData;
        if (other.CompareTag("Player") && transform.eulerAngles.z == player.eulerAngles.z) //�÷��̾ ���̺�����Ʈ�� ���� angle�� ������ ���� ��
        {
            if (stageNum == curGameData.curStageNum) // ���̺�����Ʈ�� stage�� ����� stage�� ���� ��
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
