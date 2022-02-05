using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Key : MonoBehaviour
{
    public int achievementNum;
    GameObject player;
    Rigidbody2D rigid;
    SpriteRenderer sprite;

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        rigid.AddForce(transform.up * 10.0f, ForceMode2D.Impulse);
        StartCoroutine("BeforeGetKey");
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.instance.curAchievementNum = achievementNum;
            
            GameManager.instance.respawnScene = SceneManager.GetActiveScene().buildIndex;
            GameManager.instance.respawnPos = player.transform.position;
            GameManager.instance.respawnGravityDir = Physics2D.gravity.normalized;

            int curAchievementNum = GameManager.instance.curAchievementNum;
            if (curAchievementNum >= 17 && curAchievementNum <= 20) // Stage5
            {
                GameManager.instance.UpdateShakedFloorInfo();
            }
            Destroy(gameObject);
        }
    }

    IEnumerator BeforeGetKey()
    {
        // Wait until key falls
        while (transform.InverseTransformDirection(rigid.velocity).y >= 0)
        {
            yield return null;
        }
        
        sprite.sortingLayerName = "Key";
        // enable collision between player and key
        Physics2D.IgnoreLayerCollision(10, 13, false);
    }
}
