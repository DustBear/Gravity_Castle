using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] int achievementNum;

    GameObject player;
    SpriteRenderer sprite;
    BoxCollider2D collid;

    void Awake() {
        player = GameObject.FindWithTag("Player");
        sprite = GetComponent<SpriteRenderer>();
        collid = GetComponent<BoxCollider2D>();
    }

    void Start() {
        if (GameManager.instance.curAchievementNum >= achievementNum) {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player")) {
            if (GameManager.instance.curAchievementNum == achievementNum - 1) {
                StartCoroutine("FadeOut");
            }

            GameManager.instance.respawnScene = SceneManager.GetActiveScene().buildIndex;
            GameManager.instance.respawnPos = player.transform.position;
            GameManager.instance.respawnGravityDir = Physics2D.gravity.normalized;
            int curAchievementNum = GameManager.instance.curAchievementNum;
            if (curAchievementNum >= 17 && curAchievementNum <= 20) // Stage5
            {
                GameManager.instance.UpdateShakedFloorInfo();
            }
        }
    }

    IEnumerator FadeOut() {
        for (int i = 10; i >= 0; i--) {
            Color color = sprite.color;
            color.a = i / 10.0f;
            sprite.color = color;
            yield return new WaitForSeconds(0.1f);
        }
        collid.isTrigger = true;
    }
}