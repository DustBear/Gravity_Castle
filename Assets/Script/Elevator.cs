using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public bool isStart;

    void Update()
    {
        if (isStart) {
            Vector2 targetPos = new Vector2(-80.4f, 8.06f);
            transform.position = Vector2.MoveTowards(transform.position, targetPos, 10.0f * Time.deltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        isStart = true;
    }
    
    void OnCollisionExit2D(Collision2D other) {
        isStart = false;
    }
}
