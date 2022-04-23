using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openingSceneWall : MonoBehaviour
{
    public float Speed;
    public float RunTime;

    public GameObject[] walls = new GameObject [3];

    void Start()
    {

    }


    void Update()
    {
        for(int index=0; index<3; index++)
        {
            Vector2 curPos;
            curPos = walls[index].transform.position;
            walls[index].transform.position = new Vector2(curPos.x, curPos.y - Speed * Time.deltaTime);

            if(walls[index].transform.localPosition.y <= -8)
            {
                walls[index].transform.localPosition = new Vector2(0, 16);
            }
        }
    }
}
