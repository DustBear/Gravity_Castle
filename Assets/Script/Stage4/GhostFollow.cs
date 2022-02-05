using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFollow : MonoBehaviour
{
    [SerializeField] PlayerStage4 player;
    [SerializeField] float speed;
    Vector2 targetPos;

    void Awake()
    {
        if (GameManager.instance.curAchievementNum == 15) {
            transform.position = new Vector2(-162.4f, 10.4f);
        }
        targetPos = transform.position;
    }

    void Update()
    {
        if (GameManager.instance.curAchievementNum >= 14) {
            // Timing when player start to fall after using lever
            if (player.leveringState == Player.LeveringState.changeGravityDir) {
                targetPos = player.transform.position;
            }

            // Follow
            transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            // Finish follow
            if (GameManager.instance.curAchievementNum >= 15 && (Vector2)transform.position == targetPos && Physics2D.gravity.normalized != Vector2.down) {
                player.isGhostRotating = true;
            }
            
            transform.rotation = player.transform.rotation;
        }
    }
}
