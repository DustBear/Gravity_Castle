using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorLinearEllipseShaking : FloorShaking
{
    // Linear �̵� ����
    [SerializeField] float[] linearSpeed;
    [SerializeField] Vector2[] pos;
    [SerializeField] int targetPosIdx;
    Vector2 linearPos;

    // Ellipse �̵� ����
    [SerializeField] float ellipseSpeed;
    [SerializeField] [Range(0f, 1f)] float startMovingTime; // ex) 0.5f : �� ���� �� ���¿��� ����
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
        // Linear �̵�
        linearPos = Vector2.MoveTowards(linearPos, pos[targetPosIdx], linearSpeed[targetPosIdx] * Time.deltaTime);

        // ��ǥ �������� �̵��ߴٸ� ���� ��ǥ ���� ����
        if (linearPos == pos[targetPosIdx])
        {
            targetPosIdx = (targetPosIdx + 1) % pos.Length;
        }

        // Ellipse �̵�
        ellipseTime += Time.deltaTime;
        nextPos = linearPos + new Vector2(xRad * Mathf.Sin(startMovingTime + ellipseTime * ellipseSpeed), yRad * Mathf.Cos(startMovingTime + ellipseTime * ellipseSpeed));
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
