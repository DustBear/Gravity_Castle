using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [SerializeField] GameObject targetBlackHole;
    Player player;
    Vector2 targetPos;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
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
