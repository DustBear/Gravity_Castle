using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFollow : MonoBehaviour
{
    [SerializeField] PlayerStage4 player;
    [SerializeField] float speed;
    SpriteRenderer render;
    Vector2 targetPos;

    public int activeNum; //유령이 작동 시작하는 시점 
    public int level_IncreaseNum; //유령이 강화되는 시점

    void Awake()
    {
        
        render = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (GameManager.instance.gameData.curAchievementNum == level_IncreaseNum)
        {
            transform.position = new Vector2(-162.4f, 10.4f);
        }
        targetPos = transform.position;
        StartCoroutine(IncreaseSpeed());
    }

    void Update()
    {
        if (GameManager.instance.gameData.curAchievementNum >= activeNum)
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
            if (GameManager.instance.gameData.curAchievementNum >= activeNum && (Vector2)transform.position == targetPos && Physics2D.gravity.normalized != Vector2.down)
            {
                player.isGhostRotating = true;
            }
            
            transform.rotation = player.transform.rotation;
        }
    }

    IEnumerator IncreaseSpeed()
    {
        while (GameManager.instance.gameData.curAchievementNum < level_IncreaseNum)
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
