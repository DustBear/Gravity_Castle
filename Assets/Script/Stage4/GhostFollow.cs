using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFollow : MonoBehaviour
{
    [SerializeField] PlayerStage4 player;
    [SerializeField] float speed;
    SpriteRenderer render;
    Vector2 targetPos;

    void Awake()
    {
        
        render = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (GameManager.instance.gameData.curAchievementNum == 15)
        {
            transform.position = new Vector2(-162.4f, 10.4f);
        }
        targetPos = transform.position;
        StartCoroutine(IncreaseSpeed());
    }

    void Update()
    {
        if (GameManager.instance.gameData.curAchievementNum >= 14)
        {
            // Timing when player start to fall after using lever
            if (GameManager.instance.isChangeGravityDir)
            {
                targetPos = player.transform.position;
            }
            else
            {
                // Follow
                transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            }

            // Finish follow
            if (GameManager.instance.gameData.curAchievementNum >= 15 && (Vector2)transform.position == targetPos && Physics2D.gravity.normalized != Vector2.down)
            {
                player.isGhostRotating = true;
            }
            
            transform.rotation = player.transform.rotation;
        }
    }

    IEnumerator IncreaseSpeed()
    {
        while (GameManager.instance.gameData.curAchievementNum != 15)
        {
            yield return new WaitForSeconds(5f);
        }
        speed = 5f;
        Color color = render.color;
        color.g = 0.3f;
        color.b = 0.3f;
        color.a = 1;
        render.color = color;
    }
}
