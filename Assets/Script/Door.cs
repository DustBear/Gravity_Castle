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
        if (GameManager.instance.gameData.curAchievementNum >= achievementNum) {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player")) {
            if (GameManager.instance.gameData.curAchievementNum == achievementNum - 1) {
                StartCoroutine(FadeOut());
                GameManager.instance.SaveData(achievementNum, player.transform.position);
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
        if (achievementNum == 33 && !GameManager.instance.isCliffChecked)
        {
            InputManager.instance.isInputBlocked = true;
        }
    }
}