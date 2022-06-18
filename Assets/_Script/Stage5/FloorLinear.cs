using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorLinear : MonoBehaviour
{
    [SerializeField] float[] speed;
    [SerializeField] Vector2[] pos;
    [SerializeField] int targetPosIdx;

    void Awake()
    {
        transform.position = pos[(targetPosIdx + pos.Length - 1) % pos.Length];
    }

    void Update()
    {
        // 중력 방향 전환 시 시간이 멈추므로 플랫폼도 이동 불가
        if (Player.curState == Player.States.ChangeGravityDir) return;

        // 이동
        transform.position = Vector2.MoveTowards(transform.position, pos[targetPosIdx], speed[targetPosIdx] * Time.deltaTime);
        
        // 목표 지점까지 이동했다면 다음 목표 지점 설정
        if ((Vector2)transform.position == pos[targetPosIdx])
        {
            targetPosIdx = (targetPosIdx + 1) % pos.Length;
        }
    }
}
