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
        // Linear �̵�
        nextPos = Vector2.MoveTowards(nextPos, pos[targetPosIdx], speed[targetPosIdx] * Time.deltaTime);

        // ��ǥ �������� �̵��ߴٸ� ���� ��ǥ ���� ����
        if (nextPos == pos[targetPosIdx])
        {
            targetPosIdx = (targetPosIdx + 1) % pos.Length;
        }

        return nextPos;
    }

    protected override void Idle_Update()
    {
        // �߷� ���� ��ȯ �� �ð��� ���߹Ƿ� �÷����� �̵� �Ұ�
        if (Player.curState == Player.States.ChangeGravityDir) return;

        transform.position = GetNextPos();
        base.Idle_Update();
    }

    protected override void Wait_Update()
    {
        // �߷� ���� ��ȯ �� �ð��� ���߹Ƿ� �÷����� �̵� �Ұ�
        if (Player.curState == Player.States.ChangeGravityDir) return;

        transform.position = GetNextPos();
        base.Wait_Update();
    }

    protected override void Shake_Update()
    {        
        // �߷� ���� ��ȯ �� �ð��� ���߹Ƿ� �÷����� �̵� �Ұ�
        if (Player.curState == Player.States.ChangeGravityDir) return;

        GetNextPos();
        base.Shake_Update();
    }
}
