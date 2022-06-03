using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePoint : MonoBehaviour
{
    [SerializeField] int stageNum;
    [SerializeField] int achievementNum;
    Transform player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && transform.eulerAngles.z == player.eulerAngles.z && GameManager.instance.gameData.curAchievementNum == achievementNum - 1 || GameManager.instance.gameData.curStageNum == stageNum - 1)
        {
            GameManager.instance.SaveData(achievementNum, stageNum, player.position);
        }
    }
}
