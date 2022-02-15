using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject lever;
    Vector2 moveDir;

    void OnEnable()
    {
        Vector3 playerPos = player.transform.position;
        Vector3 laserPos = transform.position;
        moveDir = playerPos - laserPos;
        float angle = Mathf.Atan2(playerPos.y - laserPos.y, playerPos.x - laserPos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, 90f + angle);
    }

    void Update()
    {
        transform.Translate(moveDir * Time.deltaTime, Space.World);
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.collider != null && other.collider.name == "Platform")
        {
            Vector2 collisionPos = other.contacts[0].point;
            lever.transform.position = collisionPos;
            lever.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
