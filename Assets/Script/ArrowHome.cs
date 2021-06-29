using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHome : MonoBehaviour
{
    public Arrow prefab;
    public float timeInterval;
    public float nextTime;
    
    public float firePower;
    public enum FireDirection {up, upright, right, downright, down, downleft, left, upleft};
    public FireDirection fireDirection;
    Vector2 fireDir;

    void Start() {
        switch (fireDirection) {
            case FireDirection.up:
                fireDir = Vector2.up;
                break;
            case FireDirection.upright:
                fireDir = (new Vector2(1, 1)).normalized;
                break;
            case FireDirection.right:
                fireDir = Vector2.right;
                break;
            case FireDirection.downright:
                fireDir = (new Vector2(1, -1)).normalized;
                break;
            case FireDirection.down:
                fireDir = Vector2.down;
                break;
            case FireDirection.downleft:
                fireDir = (new Vector2(-1, -1)).normalized;
                break;
            case FireDirection.left:
                fireDir = Vector2.left;
                break;
            case FireDirection.upleft:
                fireDir = (new Vector2(-1, 1)).normalized;
                break;
        }
    }

    void FixedUpdate() {
        if (Time.time > nextTime) {
            Arrow arrow = Instantiate(prefab, transform.position, Quaternion.identity);
            arrow.firePower = firePower;
            arrow.fireDir = fireDir;
            nextTime = Time.time + timeInterval;
        }
    }
}
