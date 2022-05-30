using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] int nextScene;
    [SerializeField] Vector2 deltaPos;
    Player player;

    void Awake() {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player") && !GameManager.instance.shouldStartAtSavePoint) {
            GameManager.instance.nextPos = (Vector2)player.transform.position + deltaPos;
            GameManager.instance.nextGravityDir = Physics2D.gravity.normalized;
            GameManager.instance.nextState = Player.curState;
            SceneManager.LoadScene(nextScene);
        }
    }
}
