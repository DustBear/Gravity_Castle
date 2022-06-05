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

    protected override void Start()
    {
        base.Start();
    }

    Vector2 GetNextPos()
    {
        // Linear 이동
        nextPos = Vector2.MoveTowards(nextPos, pos[targetPosIdx], speed[targetPosIdx] * Time.deltaTime);

        // 목표 지점까지 이동했다면 다음 목표 지점 설정
        if (nextPos == pos[targetPosIdx])
        {
            targetPosIdx = (targetPosIdx + 1) % pos.Length;
        }

        return nextPos;
    }

    protected override void Idle_Update()
    {
        // 중력 방향 전환 시 시간이 멈추므로 플랫폼도 이동 불가
        if (Player.curState == Player.States.ChangeGravityDir) return;

        transform.position = GetNextPos();
        base.Idle_Update();
    }

    protected override void Wait_Update()
    {
        // 중력 방향 전환 시 시간이 멈추므로 플랫폼도 이동 불가
        if (Player.curState == Player.States.ChangeGravityDir) return;

        transform.position = GetNextPos();
        base.Wait_Update();
    }

    protected override void Shake_Update()
    {        
        // 중력 방향 전환 시 시간이 멈추므로 플랫폼도 이동 불가
        if (Player.curState == Player.States.ChangeGravityDir) return;

        GetNextPos();
        base.Shake_Update();
    }
}
