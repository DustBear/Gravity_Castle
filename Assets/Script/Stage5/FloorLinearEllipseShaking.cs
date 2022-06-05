using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorLinearEllipseShaking : FloorShaking
{
    // Linear 이동 관련
    [SerializeField] float[] linearSpeed;
    [SerializeField] Vector2[] pos;
    [SerializeField] int targetPosIdx;
    Vector2 linearPos;

    // Ellipse 이동 관련
    [SerializeField] float ellipseSpeed;
    [SerializeField] [Range(0f, 1f)] float startMovingTime; // ex) 0.5f : 반 바퀴 돈 상태에서 시작
    [SerializeField] float xRad, yRad;
    float ellipseTime;

    protected override void Awake()
    {
        base.Awake();
        transform.position = pos[(targetPosIdx + pos.Length - 1) % pos.Length];
        startMovingTime *= 3.141592f * 2f;
        linearPos = pos[0];
    }

    protected override void Start()
    {
        base.Start();
    }

    Vector2 GetNextPos()
    {
        // Linear 이동
        linearPos = Vector2.MoveTowards(linearPos, pos[targetPosIdx], linearSpeed[targetPosIdx] * Time.deltaTime);

        // 목표 지점까지 이동했다면 다음 목표 지점 설정
        if (linearPos == pos[targetPosIdx])
        {
            targetPosIdx = (targetPosIdx + 1) % pos.Length;
        }

        // Ellipse 이동
        ellipseTime += Time.deltaTime;
        nextPos = linearPos + new Vector2(xRad * Mathf.Sin(startMovingTime + ellipseTime * ellipseSpeed), yRad * Mathf.Cos(startMovingTime + ellipseTime * ellipseSpeed));
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
