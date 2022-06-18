using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostLinear : MonoBehaviour
{
    Player player;
    [SerializeField] float[] speed;
    [SerializeField] Vector2[] pos;
    [SerializeField] int targetPosIdx;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        transform.position = pos[(targetPosIdx + pos.Length - 1) % pos.Length];
    }

    void Update()
    {
        if (Player.curState != Player.States.ChangeGravityDir)
        {
            transform.position = Vector2.MoveTowards(transform.position, pos[targetPosIdx], speed[targetPosIdx] * Time.deltaTime);
            if ((Vector2)transform.position == pos[targetPosIdx])
            {
                targetPosIdx = (targetPosIdx + 1) % pos.Length;
            }
        }
        
        transform.rotation = player.transform.rotation;
    }
}
