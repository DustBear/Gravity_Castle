using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public bool isNextStage;
    public int nextScene;
    public Vector2 deltaPos;
    Player player;

    void Awake() {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player")) {
            GameManager.instance.nextPos.x = player.transform.position.x + deltaPos.x;
            GameManager.instance.nextPos.y = player.transform.position.y + deltaPos.y;
            GameManager.instance.nextRot = player.transform.rotation;
            GameManager.instance.nextGravity = Physics2D.gravity;
            GameManager.instance.nextAfterRotating = player.afterRotating;
            GameManager.instance.nextGravityDir = player.gravityDirection;
            GameManager.instance.nextIsJumping = player.isJumping;
            GameManager.instance.nextIsRoping = player.isRoping;

            if (isNextStage) {
                GameManager.instance.curStage++;
                GameManager.instance.curState = 0;
                GameManager.instance.isOpenKeyBox1 = false;
                GameManager.instance.isOpenKeyBox2 = false;
                GameManager.instance.isGetKey1 = false;
                GameManager.instance.isGetKey2 = false;
                GameManager.instance.isOpenDoor1 = false;
                GameManager.instance.isOpenDoor2 = false;
            }

            SceneManager.LoadScene(nextScene);
        }
    }
}
