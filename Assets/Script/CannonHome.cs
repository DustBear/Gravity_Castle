using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonHome : MonoBehaviour
{
    public Cannon prefab;
    public float timeInterval;
    public float nextTime;
    
    public float firePower;
    public enum FireDirection {up, upright, right, downright, down, downleft, left, upleft};
    public FireDirection fireDirection;
    Vector2 fireDir;
    Quaternion fireRot;

    void Start() {
        switch (fireDirection) {
            case FireDirection.up:
                fireDir = Vector2.up;
                fireRot = Quaternion.Euler(0, 0, 0);
                break;
            case FireDirection.upright:
                fireDir = (new Vector2(1, 1)).normalized;
                fireRot = Quaternion.Euler(0, 0, 315);
                break;
            case FireDirection.right:
                fireDir = Vector2.right;
                fireRot = Quaternion.Euler(0, 0, 270);
                break;
            case FireDirection.downright:
                fireDir = (new Vector2(1, -1)).normalized;
                fireRot = Quaternion.Euler(0, 0, 225);
                break;
            case FireDirection.down:
                fireDir = Vector2.down;
                fireRot = Quaternion.Euler(0, 0, 180);
                break;
            case FireDirection.downleft:
                fireDir = (new Vector2(-1, -1)).normalized;
                fireRot = Quaternion.Euler(0, 0, 135);
                break;
            case FireDirection.left:
                fireDir = Vector2.left;
                fireRot = Quaternion.Euler(0, 0, 90);
                break;
            case FireDirection.upleft:
                fireDir = (new Vector2(-1, 1)).normalized;
                fireRot = Quaternion.Euler(0, 0, 45);
                break;
        }
    }

    void FixedUpdate() {
        if (Time.time > nextTime) {
            Cannon cannon = Instantiate(prefab, transform.position, Quaternion.identity);
            cannon.firePower = firePower;
            cannon.fireDir = fireDir;
            cannon.fireRot = fireRot;
            nextTime = Time.time + timeInterval;
        }
    }
}
