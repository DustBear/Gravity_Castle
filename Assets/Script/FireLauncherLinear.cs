using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLauncherLinear : MonoBehaviour
{
    Rigidbody2D rigid;
    public float[] speed;
    public Vector2[] pos;
    public int targetPosIdx;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.position = pos[(targetPosIdx + pos.Length - 1) % pos.Length];
    }

    void Update()
    {
        rigid.position = Vector2.MoveTowards(transform.position, pos[targetPosIdx], speed[targetPosIdx] * Time.deltaTime);
        if ((Vector2)transform.position == pos[targetPosIdx]) {
            targetPosIdx = (targetPosIdx + 1) % pos.Length;
        }
    }
}
