using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEllipse : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] bool counterClockWise;
    [SerializeField] float speed;
    [SerializeField] float startTime;
    [SerializeField] float xRad, yRad;
    float time;
    Vector2 centerPos;

    void Awake()
    {
        time = startTime;
        centerPos = transform.position;
    }

    void Update()
    {
        if (!GameManager.instance.isChangeGravityDir)
        {
            time += Time.deltaTime;
            if (!counterClockWise)
            {
                transform.position = centerPos + new Vector2(xRad * Mathf.Sin(time * speed), yRad * Mathf.Cos(time * speed));
            }
            else
            {
                transform.position = centerPos + new Vector2(xRad * Mathf.Sin(-time * speed), yRad * Mathf.Cos(-time * speed));
            }
        }
        
        transform.rotation = player.transform.rotation;
    }
}
