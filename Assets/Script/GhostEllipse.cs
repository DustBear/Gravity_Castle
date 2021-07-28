using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEllipse : MonoBehaviour
{
    Rigidbody2D rigid;
    Player player;
    public float speed;
    float time;
    public float startTime;
    Vector2 centerPos;
    public float xRad, yRad;
    public bool counterClockWise;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        transform.rotation = player.transform.rotation;
        time = startTime;
        centerPos = transform.position;
    }

    void Update()
    {
        time += Time.deltaTime;
        if (!counterClockWise) {
            rigid.position = centerPos + new Vector2(xRad * Mathf.Sin(time * speed), yRad * Mathf.Cos(time * speed));
        }
        else {
            rigid.position = centerPos + new Vector2(xRad * Mathf.Sin(-time * speed), yRad * Mathf.Cos(-time * speed));
        }
        transform.rotation = player.transform.rotation;
    }
}
