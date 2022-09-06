using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floatingLanton : MonoBehaviour
{
    GameObject playerObj;
    Vector3 playerDir;

    public float period;
    public float sinForce;
    Vector3 initPos;

    private void Awake()
    {
        playerObj = GameObject.Find("Player");
    }

    void Start()
    {
        playerDir = playerObj.transform.up;
        initPos = transform.position;
        rotateCorrect();
    }

    void Update()
    {
        sinMove();

        //플레이어의 방향이 바뀔때만 호출 
        if (playerDir != playerObj.transform.up)
        {
            rotateCorrect();
        }

        playerDir = playerObj.transform.up;
    }

    void rotateCorrect()
    {
        transform.rotation = playerObj.transform.rotation;
    }

    void sinMove()
    {
        transform.position = initPos + Mathf.Sin(Time.time/period) * transform.up;
    }
}
