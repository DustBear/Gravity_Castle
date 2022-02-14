using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [SerializeField] GameObject targetBlackHole;
    [SerializeField] PlayerStage8 player;
    Vector2 targetPos;

    void Awake()
    {
        targetPos = targetBlackHole.transform.position;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other != null && other.CompareTag("Player") && !player.isBlackHole)
        {
            player.isBlackHole = true;
            StartCoroutine(player.MoveToBlackHole(transform.position, targetPos));
        }
    }
}
