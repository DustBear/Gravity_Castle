using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStage4 : Player
{
    [HideInInspector] public bool isGhostRotating;

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
            if (!isGhostRotating)
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
                    if (ropingState == RopingState.idle)
                    {
                        Walk();
                    }
                }
            }
            else
            {
                GhostUsingLever();
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

        if (other.CompareTag("Ghost")) {
            SpriteRenderer renderer = other.GetComponent<SpriteRenderer>();
            if (renderer.color.a != 0f)
            {
                GameManager.instance.isDie = true;
                UIManager.instance.FadeOut();
            }
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
    }

    protected override void IsGrounded()
    {
       base.IsGrounded();
    }

    void GhostUsingLever() {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 8.0f);
        int angle = Mathf.RoundToInt(transform.eulerAngles.z);
        if (angle == 0 || angle == 360 ) {
            transform.eulerAngles = Vector3.zero;
            Physics2D.gravity = new Vector2(0, -9.8f);
            rigid.gravityScale = 2;
            if (isGrounded) {
                isGhostRotating = false;
            }
        }
        else {
            rigid.gravityScale = 0;
            rigid.velocity = Vector2.zero;
        }
    }
}
