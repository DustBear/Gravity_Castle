using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject lever;
    [SerializeField] GameObject effect;
    Vector2 moveDir;

    void OnEnable()
    {
        Vector3 playerPos = player.transform.position;
        Vector3 laserPos = transform.position;
        moveDir = playerPos - laserPos;
        float angle = Mathf.Atan2(playerPos.y - laserPos.y, playerPos.x - laserPos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Update()
    {
        transform.Translate(moveDir * 1.5f * Time.deltaTime, Space.World);
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.collider != null)
        {
            Vector2 collisionPos = other.contacts[0].point;
            effect.transform.position = collisionPos;
            effect.SetActive(true);
            if (other.collider.name == "LeftPlatform")
            {
                lever.transform.position = new Vector2(-126.5f, collisionPos.y);
                lever.transform.eulerAngles = Vector3.forward * 270f;
                lever.SetActive(true);
            }
            else if (other.collider.name == "RightPlatform")
            {
                lever.transform.position = new Vector2(-90.5f, collisionPos.y);
                lever.transform.eulerAngles = Vector3.forward * 90f;
                lever.SetActive(true);
            }
            else if (other.collider.name == "UpPlatform")
            {
                lever.transform.position = new Vector2(collisionPos.x, 40.5f);
                lever.transform.eulerAngles = Vector3.forward * 180f;
                lever.SetActive(true);
            }
            else if (other.collider.name == "DownPlatform")
            {
                lever.transform.position = new Vector2(collisionPos.x, 7.5f);
                lever.transform.eulerAngles = Vector3.forward * 0f;
                lever.SetActive(true);
            }
            gameObject.SetActive(false);
        }
    }
}
