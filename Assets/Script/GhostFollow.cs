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
        if (gameManager.isGetKey2) {
            rigid.position = new Vector2(-162.4f, 10.4f);
        }
        transform.rotation = player.transform.rotation;
        targetPos = transform.position;
    }

    void Update()
    {
        if (gameManager.isOpenDoor1) {
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

            // follow
            rigid.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            transform.rotation = player.transform.rotation;
            // finish follow
            if (gameManager.isGetKey2 && (Vector2)transform.position == targetPos && player.gravityDirection != GameManager.GravityDirection.down) {
                player.isGhostRotating = true;
            }
        }
    }
}
