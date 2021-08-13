using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorEllipse : MonoBehaviour
{
    public float speed;
    float time;
    public float startTime;
    Vector2 centerPos;
    public float xRad, yRad;
    public bool counterClockWise;

    void Awake()
    {
        time = startTime;
        centerPos = transform.position;
    }

    void Update()
    {
        time += Time.deltaTime;
        if (!counterClockWise) {
            transform.position = centerPos + new Vector2(xRad * Mathf.Sin(time * speed), yRad * Mathf.Cos(time * speed));
        }
        else {
            transform.position = centerPos + new Vector2(xRad * Mathf.Sin(-time * speed), yRad * Mathf.Cos(-time * speed));
        }
    }
}
