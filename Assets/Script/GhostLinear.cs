using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostLinear : MonoBehaviour
{
    Rigidbody2D rigid;
    Player player;
    public float[] speed;
    public Vector2[] pos;
    public int targetPosIdx;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        rigid.position = pos[(targetPosIdx + pos.Length - 1) % pos.Length];
        transform.rotation = player.transform.rotation;
    }

    void Update()
    {
        rigid.position = Vector2.MoveTowards(transform.position, pos[targetPosIdx], speed[targetPosIdx] * Time.deltaTime);
        if ((Vector2)transform.position == pos[targetPosIdx]) {
            targetPosIdx = (targetPosIdx + 1) % pos.Length;
        }
        transform.rotation = player.transform.rotation;
    }
}
