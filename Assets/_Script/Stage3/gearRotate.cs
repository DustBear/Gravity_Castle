using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gearRotate : MonoBehaviour
{
    public float rotateSpeed;
    public int rotateDirection; //1�̸� �ð���� ȸ�� -1�̸� �ݽð���� ȸ�� 
    void Start()
    {
        
    }

    
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, rotateSpeed * Time.deltaTime * rotateDirection));
    }
}
