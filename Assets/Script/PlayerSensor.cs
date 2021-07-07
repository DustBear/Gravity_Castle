using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSensor : MonoBehaviour
{
    public bool collidePlayer;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            collidePlayer = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            collidePlayer = false;
        }
    }
}
