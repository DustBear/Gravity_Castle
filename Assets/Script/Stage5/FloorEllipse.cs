using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorEllipse : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] [Range(0f, 1f)] float startTime; // ex) 0.5f : �� ���� �� ���¿��� ����
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
        // �߷� ���� ��ȯ �� �ð��� ���߹Ƿ� �÷����� �̵� �Ұ�
        if (Player.curState == Player.States.ChangeGravityDir) return;

        // �̵�
        time += Time.deltaTime;
        transform.position = centerPos + new Vector2(xRad * Mathf.Sin(startTime + time * speed), yRad * Mathf.Cos(startTime + time * speed));
    }
}
