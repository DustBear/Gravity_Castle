using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStage3 : Player 
{
    [SerializeField] float windForce;
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
            // Vector2 locVel = transform.InverseTransformDirection(rigid.velocity);
            // if (locVel.y <= -20f)
            // {
            //     rigid.velocity = transform.TransformDirection(new Vector2(locVel.x, -20f));
            // }
            IsGrounded();
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
                rigid.AddForce(Vector2.up * windForce, ForceMode2D.Force);
            }
            else if (other.CompareTag("DownWind")) {
                rigid.AddForce(Vector2.down * windForce, ForceMode2D.Force);
            }
            else if (other.CompareTag("RightWind")) {
                rigid.AddForce(Vector2.right * windForce, ForceMode2D.Force);
            }
            else if (other.CompareTag("LeftWind")) {
                rigid.AddForce(Vector2.left * windForce, ForceMode2D.Force);
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

    protected override void IsGrounded()
    {
        RaycastHit2D rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.8f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 3 | 1 << 12 | 1 << 16);
        isGrounded = rayHit.collider != null;
    }
}
