using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorEllipse : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] [Range(0f, 1f)] float startTime; // ex) 0.5f : 반 바퀴 돈 상태에서 시작
    [SerializeField] float xRad, yRad;
    float time;
    Vector2 centerPos;

    void Awake()
    {
        startTime *= 3.141592f * 2f;
        centerPos = transform.position;
    }

    void Update()
    {
        // 중력 방향 전환 시 시간이 멈추므로 플랫폼도 이동 불가
        if (Player.curState == Player.States.ChangeGravityDir) return;

        // 이동
        time += Time.deltaTime;
        transform.position = centerPos + new Vector2(xRad * Mathf.Sin(startTime + time * speed), yRad * Mathf.Cos(startTime + time * speed));
    }
}
