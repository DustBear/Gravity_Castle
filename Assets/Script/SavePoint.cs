using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePoint : MonoBehaviour
{
    [SerializeField] int achievementNum;
    [SerializeField] Transform player;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && transform.eulerAngles.z == player.eulerAngles.z)
        {
            GameManager.instance.curAchievementNum = achievementNum;
            GameManager.instance.respawnScene = SceneManager.GetActiveScene().buildIndex;
            GameManager.instance.respawnPos = player.position;
            GameManager.instance.respawnGravityDir = Physics2D.gravity.normalized;
            if (achievementNum >= 17 && achievementNum <= 20) // Stage5
            {
                GameManager.instance.UpdateShakedFloorInfo();
            }
            DataManager.instance.SaveData();
        }
    }
}
