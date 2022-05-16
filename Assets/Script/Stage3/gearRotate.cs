using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gearRotate : MonoBehaviour
{
    public float rotateSpeed;
    public int rotateDirection; //1이면 시계방향 회전 -1이면 반시계방향 회전 
    void Start()
    {
        
    }

    
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, rotateSpeed * Time.deltaTime * rotateDirection));
    }
}
