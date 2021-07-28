using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFollow : MonoBehaviour
{
    GameManager gameManager;
    Rigidbody2D rigid;
    Player player;
    public float speed;
    Vector2 targetPos;
    bool isFinishRotating = true;

    void Awake()
    {   
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        transform.rotation = player.transform.rotation;
        targetPos = transform.position;
    }

    void Update()
    {
        // timing when player start to fall after using lever
        if (player.afterRotating) {
            if (!isFinishRotating) {
                isFinishRotating = true;
                targetPos = player.startingPos;
            }
        }
        else {
            isFinishRotating = false;
        }
        rigid.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        transform.rotation = player.transform.rotation;

        if (gameManager.isGetKey2 && (Vector2)transform.position == targetPos && player.gravityDirection != GameManager.GravityDirection.down) {
            player.isGhostRotating = true;
        }
    }
}
