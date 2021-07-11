using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public bool isNextStage;
    public int nextScene;
    public Vector2 deltaPos;
    GameManager gameManager;
    Player player;
    Rigidbody2D playerRigid;

    void Awake() {
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        playerRigid = player.GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D other) {
        gameManager.nextPos.x = player.transform.position.x + deltaPos.x; 
        gameManager.nextPos.y = player.transform.position.y + deltaPos.y;
        gameManager.nextRot = player.transform.rotation;
        gameManager.nextGravity = Physics2D.gravity;
        gameManager.nextAfterRotating = player.afterRotating;
        gameManager.nextGravityDir = player.gravityDirection;
        gameManager.nextGravityScale = playerRigid.gravityScale;
        gameManager.nextIsJumping = player.isJumping;
        gameManager.nextIsRoping = player.isRoping;
        gameManager.nextIsCollideRope = player.isCollideRope;

        if (isNextStage) {
            gameManager.curStage++;
            gameManager.curState = 0;
            gameManager.isOpenKeyBox1 = false;
            gameManager.isOpenKeyBox2 = false;
            gameManager.isGetKey1 = false;
            gameManager.isGetKey2 = false;
            gameManager.isOpenDoor1 = false;
            gameManager.isOpenDoor2 = false;
        }
        
        SceneManager.LoadScene(nextScene);
    }
}
