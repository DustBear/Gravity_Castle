using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorLinearEllipseShaking : FloorShaking
{    
    [SerializeField] float[] speed;
    [SerializeField] Vector2[] pos;
    [SerializeField] int targetPosIdx;

    [SerializeField] float startMovingTime;
    [SerializeField] float ellipseSpeed;
    [SerializeField] float xRad, yRad;
    [SerializeField] bool counterClockWise;

    float movingTime;

    protected override void Awake()
    {
        base.Awake();
        transform.position = pos[(targetPosIdx + pos.Length - 1) % pos.Length];
        movingTime = startMovingTime;
        nextPos = pos[0];
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Idle()
    {
        if (!GameManager.instance.isChangeGravityDir)
        {
            // linear moved position
            nextPos = Vector2.MoveTowards(nextPos, pos[targetPosIdx], speed[targetPosIdx] * Time.deltaTime);
            // add ellipse moved position
            movingTime += Time.deltaTime;
            if (!counterClockWise)
            {
                transform.position = nextPos + new Vector2(xRad * Mathf.Sin(movingTime * ellipseSpeed), yRad * Mathf.Cos(movingTime * ellipseSpeed));
            }
            else
            {
                transform.position = nextPos + new Vector2(xRad * Mathf.Sin(-movingTime * ellipseSpeed), yRad * Mathf.Cos(-movingTime * ellipseSpeed));
            }
            // if floor arrive target position, convert targetPosIdx to next targetPosIdx
            if (nextPos == pos[targetPosIdx])
            {
                targetPosIdx = (targetPosIdx + 1) % pos.Length;
            }
        }
        base.Idle();
    }

    protected override void Waiting()
    {
        nextPos = Vector2.MoveTowards(nextPos, pos[targetPosIdx], speed[targetPosIdx] * Time.deltaTime);
        movingTime += Time.deltaTime;
        if (!counterClockWise)
        {
            transform.position = nextPos + new Vector2(xRad * Mathf.Sin(movingTime * ellipseSpeed), yRad * Mathf.Cos(movingTime * ellipseSpeed));
        }
        else
        {
            transform.position = nextPos + new Vector2(xRad * Mathf.Sin(-movingTime * ellipseSpeed), yRad * Mathf.Cos(-movingTime * ellipseSpeed));
        }

        if (nextPos == pos[targetPosIdx])
        {
            targetPosIdx = (targetPosIdx + 1) % pos.Length;
        }
        base.Waiting();
    }

    protected override void Shaked()
    {
        nextPos = Vector2.MoveTowards(nextPos, pos[targetPosIdx], speed[targetPosIdx] * Time.deltaTime);
        movingTime += Time.deltaTime;
        Vector2 tmpPos = nextPos;
        if (!counterClockWise)
        {
            nextPos += new Vector2(xRad * Mathf.Sin(movingTime * ellipseSpeed), yRad * Mathf.Cos(movingTime * ellipseSpeed));
        }
        else
        {
            nextPos += new Vector2(xRad * Mathf.Sin(-movingTime * ellipseSpeed), yRad * Mathf.Cos(-movingTime * ellipseSpeed));
        }

        if (tmpPos == pos[targetPosIdx])
        {
            targetPosIdx = (targetPosIdx + 1) % pos.Length;
        }
        base.Shaked();
        nextPos = tmpPos;
    }
}
