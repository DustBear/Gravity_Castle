using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStage3 : Player 
{
    bool isHorizontalWind;
    bool isVerticalWind;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if (!GameManager.instance.isDie)
        {
            if (!isJumping && ropingState == RopingState.idle)
            {
                Lever();
            }
            if (leveringState == LeveringState.idle)
            {
                Rope();
                if (ropingState != RopingState.access)
                {
                    Jump();
                }
                if (ropingState == RopingState.idle && (!isHorizontalWind || Physics2D.gravity.x != 0f)
                    && (!isVerticalWind || Physics2D.gravity.y != 0f))
                {
                    Walk();
                }
            }
        }
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        base.OnCollisionEnter2D(other);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.CompareTag("RightWind") || other.CompareTag("LeftWind")) {
            isHorizontalWind = true;
        }
        if (other.CompareTag("UpWind") || other.CompareTag("DownWind")) {
            isVerticalWind = true;
        }
    }

    void OnTriggerStay2D(Collider2D other) {
        if (ropingState == RopingState.idle) {
            if (other.CompareTag("UpWind")) {
                rigid.AddForce(Vector2.up * 50.0f, ForceMode2D.Force);
            }
            else if (other.CompareTag("DownWind")) {
                rigid.AddForce(Vector2.down * 50.0f, ForceMode2D.Force);
            }
            else if (other.CompareTag("RightWind")) {
                rigid.AddForce(Vector2.right * 50.0f, ForceMode2D.Force);
            }
            else if (other.CompareTag("LeftWind")) {
                rigid.AddForce(Vector2.left * 50.0f, ForceMode2D.Force);
            }
        }
    }

    protected override void OnTriggerExit2D(Collider2D other) {
        base.OnTriggerExit2D(other);
        if (other.CompareTag("RightWind") || other.CompareTag("LeftWind")) {
            isHorizontalWind = false;
        }
        if (other.CompareTag("UpWind") || other.CompareTag("DownWind")) {
            isVerticalWind = false;
        }
    }

    protected override bool IsGrounded()
    {
        RaycastHit2D rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.6f, 0.1f), transform.eulerAngles.z, -transform.up, 0.8f, 1 << 3 | 1 << 12 | 1 << 16);
        return rayHit.collider != null;
    }
}
