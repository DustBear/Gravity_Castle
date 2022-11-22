using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floatingLanton : MonoBehaviour
{
    GameObject playerObj;
    Vector3 playerDir;

    Vector3 initPos;

    private void Awake()
    {
        playerObj = GameObject.Find("Player");
    }

    void Start()
    {
        if(playerObj != null)
        {
            playerDir = playerObj.transform.up;
        }

        initPos = transform.position;
        rotateCorrect();
    }

    void Update()
    {
        if (playerObj == null) return;
        //�÷��̾��� ������ �ٲ𶧸� ȣ�� 
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
}
