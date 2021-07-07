using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySensor : MonoBehaviour
{
    GameManager gameManager;
    PlayerSensor playerSensor;
    bool collideKey1;
    bool collideKey2;

    void Awake() {
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        playerSensor = transform.parent.GetChild(3).GetComponent<PlayerSensor>();
    }

    void Update() {
        if (playerSensor.collidePlayer) {
            if (collideKey1) {
                gameManager.isGetKey1 = true;
                gameManager.curState = 1;
            }
            if (collideKey2) {
                gameManager.isGetKey2 = true;
                gameManager.curState = 3;
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Key1")) {
            collideKey1 = true;
        }
        if (other.CompareTag("Key2")) {
            collideKey2 = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Key1")) {
            collideKey1 = false;
        }
        if (other.CompareTag("Key2")) {
            collideKey2 = false;
        }
    }
}
