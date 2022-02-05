using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStage6 : Player 
{
    bool isRayHitIce;

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
        base.Update();
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        base.OnCollisionEnter2D(other);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
    }

    protected override void Walk()
    {
         // Move
        Vector2 locVel = transform.InverseTransformDirection(rigid.velocity);
        // on ice
        if (isRayHitIce && InputManager.instance.horizontal == 0f) {
            locVel = new Vector2(Vector2.Lerp(locVel, Vector2.zero, Time.deltaTime * 8f).x , locVel.y);
        }
        // not ice
        else {
            locVel = new Vector2(InputManager.instance.horizontal * walkSpeed, locVel.y);
        }
        rigid.velocity = transform.TransformDirection(locVel);

        // Animation
        if (InputManager.instance.horizontal != 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
        if (InputManager.instance.horizontal == 1f)
        {
            sprite.flipX = false;
        }
        else if (InputManager.instance.horizontal == -1f)
        {
            sprite.flipX = true;
        }
    }

    protected override bool IsGrounded()
    {
        RaycastHit2D rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.6f, 0.1f), transform.eulerAngles.z, -transform.up, 0.8f, 1 << 3 | 1 << 16 | 1 << 19);
        
        // IcePlatform
        RaycastHit2D rayHitIcePlatform = Physics2D.BoxCast(transform.position, new Vector2(0.6f, 0.1f), transform.eulerAngles.z, -transform.up, 0.8f, 1 << 9);
        if (rayHitIcePlatform.collider != null) {
            isRayHitIce = true;
        }
        else {
            isRayHitIce = false;
        }
        return rayHit.collider != null || rayHitIcePlatform.collider;
    }
}
