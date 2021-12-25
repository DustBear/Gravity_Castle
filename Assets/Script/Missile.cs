using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    Player player;
    Vector3 dir;

    void Awake() {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        dir = (player.transform.position - this.transform.position).normalized;
    }

    void Start() {
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f);
    }

    void Update() {
        this.transform.Translate(dir * Time.deltaTime * 5.0f, Space.World);
        RaycastHit2D rayHit = Physics2D.CircleCast(transform.position, 0.5f, Vector2.zero, 0f, 1 << 3);
        if (rayHit.collider != null) {
            Destroy(gameObject);
        }
    }
}
