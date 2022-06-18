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
        if (other.CompareTag("Player") && 
            transform.eulerAngles.z == player.eulerAngles.z && 
            GameManager.instance.gameData.curAchievementNum == achievementNum - 1 || GameManager.instance.gameData.curStageNum == stageNum - 1)
        {
            //�÷��̾ ���̺�����Ʈ�� ���� angle�� ������ �ְ�, �÷��̾ �� ���̺�����Ʈ�� �ٷ� �� ���̺�����Ʈ���� Ȱ��ȭ������ ���� �۵� 
            GameManager.instance.SaveData(achievementNum, stageNum, respawnPos);
        }
    }
}
