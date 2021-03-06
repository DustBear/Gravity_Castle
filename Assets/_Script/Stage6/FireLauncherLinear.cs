using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLauncherLinear : MonoBehaviour
{
    [SerializeField] Vector2[] pos;
    [SerializeField] float[] speed;
    [SerializeField] int targetPosIdx;
    [HideInInspector] public int buttonNum;

    void Awake()
    {
        transform.position = pos[(targetPosIdx + pos.Length - 1) % pos.Length];
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, pos[targetPosIdx], speed[targetPosIdx] * Time.deltaTime);
        GameManager.instance.gameData.storedPos[buttonNum] = transform.position;
        if ((Vector2)transform.position == pos[targetPosIdx]) {
            targetPosIdx = (targetPosIdx + 1) % pos.Length;
        }
    }
}
