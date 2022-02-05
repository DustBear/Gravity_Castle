using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorLinearShaking : FloorShaking
{
    [SerializeField] float[] speed;
    [SerializeField] Vector2[] pos;
    [SerializeField] int targetPosIdx;

    protected override void Awake()
    {
        base.Awake();
        transform.position = pos[(targetPosIdx + pos.Length - 1) % pos.Length];
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Idle()
    {
        nextPos = Vector2.MoveTowards(nextPos, pos[targetPosIdx], speed[targetPosIdx] * Time.deltaTime);
        // if floor arrive target position, convert targetPosIdx to next targetPosIdx
        if (Vector2.Distance(nextPos, pos[targetPosIdx]) < 0.1f)
        {
            targetPosIdx = (targetPosIdx + 1) % pos.Length;
        }
        transform.position = nextPos;
        base.Idle();
    }

    protected override void Waiting()
    {
        nextPos = Vector2.MoveTowards(nextPos, pos[targetPosIdx], speed[targetPosIdx] * Time.deltaTime);
        if (nextPos == pos[targetPosIdx])
        {
            targetPosIdx = (targetPosIdx + 1) % pos.Length;
        }
        transform.position = nextPos;
        base.Waiting();
    }

    protected override void Shaked()
    {
        nextPos = Vector2.MoveTowards(nextPos, pos[targetPosIdx], speed[targetPosIdx] * Time.deltaTime);
        if (nextPos == pos[targetPosIdx])
        {
            targetPosIdx = (targetPosIdx + 1) % pos.Length;
        }
        base.Shaked();
    }
}
