using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] int nextStage;
    [SerializeField] int nextScene;
    [SerializeField] Vector2 deltaPos;
    Player player;

    void Awake() {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player")) {
            GameManager.instance.nextPos.x = player.transform.position.x + deltaPos.x;
            GameManager.instance.nextPos.y = player.transform.position.y + deltaPos.y;
            GameManager.instance.nextGravityDir = Physics2D.gravity.normalized;
            GameManager.instance.nextRopingState = player.ropingState;
            GameManager.instance.nextLeveringState = player.leveringState;
            GameManager.instance.nextIsJumping = player.isJumping;
            SceneManager.LoadScene(nextScene);
        }
    }
}
