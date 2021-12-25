using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLauncherLasting : MonoBehaviour
{
    public bool isVertical;
    public GameObject fire;
    public Vector2 leftPos, rightPos;
    public Vector2 finalPos;
    public float curSpeed;
    public float deltaSpeed;
    float startTime;
    
    void Start() {
        if (!isVertical) {
            for (float f = leftPos.x; f <= rightPos.x; f += 0.5f) {
                GameObject newFire = Instantiate(fire, new Vector2(f, transform.position.y), Quaternion.Euler(0f, 0f, 0f));
                newFire.transform.parent = gameObject.transform;
                newFire.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
        else {
            for (float f = leftPos.y; f >= rightPos.y; f -= 0.5f) {
                GameObject newFire = Instantiate(fire, new Vector2(transform.position.x, f), Quaternion.Euler(0f, 0f, 0f));
                newFire.transform.parent = gameObject.transform;
                newFire.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }

        startTime = Time.time;
    }

    void Update() {
        transform.position = Vector2.MoveTowards(transform.position, finalPos, Time.deltaTime * curSpeed);
        if (Time.time - startTime > 0.1f) {
            curSpeed += deltaSpeed;
            startTime = Time.time;
        }
    }
}