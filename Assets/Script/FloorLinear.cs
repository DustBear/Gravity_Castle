using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorLinear : MonoBehaviour
{
    public float[] speed;
    public Vector2[] pos;
    public int targetPosIdx;

    void Awake()
    {
        transform.position = pos[(targetPosIdx + pos.Length - 1) % pos.Length];
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, pos[targetPosIdx], speed[targetPosIdx] * Time.deltaTime);
        if ((Vector2)transform.position == pos[targetPosIdx]) {
            targetPosIdx = (targetPosIdx + 1) % pos.Length;
        }
    }
}
