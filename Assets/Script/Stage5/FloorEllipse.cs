using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorEllipse : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float startTime;
    [SerializeField] float xRad, yRad;
    [SerializeField] bool counterClockWise;
    float time;
    Vector2 centerPos;

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
