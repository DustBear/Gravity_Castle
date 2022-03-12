using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Key : MonoBehaviour
{
    public int achievementNum;
    [SerializeField] Transform player;
    Rigidbody2D rigid;
    SpriteRenderer sprite;

    void Awake()
    {
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
            GameManager.instance.respawnPos = player.position;
            GameManager.instance.respawnGravityDir = Physics2D.gravity.normalized;
            if (achievementNum >= 17 && achievementNum <= 20) // Stage5
            {
                GameManager.instance.UpdateShakedFloorInfo();
            }
            else if (achievementNum >= 21 && achievementNum <= 24) // Stage6
            {
                GameManager.instance.UpdateIceInfo();
                GameManager.instance.UpdateDetectorInfo();
                GameManager.instance.UpdateButtonInfo();
                GameManager.instance.UpdatePosInfo();
            }
            DataManager.instance.SaveData();
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
        
        // Enable collision between player and key
        Physics2D.IgnoreLayerCollision(13, 10, false);
    }
}
