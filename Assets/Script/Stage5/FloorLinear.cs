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
        if (!GameManager.instance.isChangeGravityDir)
        {
            transform.position = Vector2.MoveTowards(transform.position, pos[targetPosIdx], speed[targetPosIdx] * Time.deltaTime);
            if ((Vector2)transform.position == pos[targetPosIdx])
            {
                targetPosIdx = (targetPosIdx + 1) % pos.Length;
            }
        }
    }
}
