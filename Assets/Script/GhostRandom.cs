using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostRandom : MonoBehaviour
{
    Rigidbody2D rigid;
    public Vector2 startPos;
    public Vector2 endPos;
    Vector2 targetPos;
    float time;
    public float speed;

    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        targetPos = new Vector2(Random.Range(startPos.x, endPos.x), Random.Range(startPos.y, endPos.y));
    }

    void Update()
    {
        time += Time.deltaTime;
        if (time > 1.0f) {
            targetPos = new Vector2(Random.Range(startPos.x, endPos.x), Random.Range(startPos.y, endPos.y));
            time = 0.0f;
        }
        rigid.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }
}
