using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStage5 : Player 
{
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
        if (other.gameObject.tag == "Spike")
        {
            GameManager.instance.InitShakedFloorInfo();
            GameManager.instance.isDie = true;
            UIManager.instance.FadeOut();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }

    protected override void OnTriggerExit2D(Collider2D other) {
        base.OnTriggerExit2D(other);
    }

    protected override bool IsGrounded()
    {
        RaycastHit2D rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.6f, 0.1f), transform.eulerAngles.z, -transform.up, 0.8f, 1 << 3 | 1 << 12);

        // MovingPlatform
        RaycastHit2D rayHitMovingPlatform = Physics2D.BoxCast(transform.position, new Vector2(0.6f, 0.1f), transform.eulerAngles.z, -transform.up, 0.8f, 1 << 16);
        if (rayHitMovingPlatform.collider != null && leveringState != LeveringState.changeGravityDir)
        {
            transform.parent = rayHitMovingPlatform.collider.transform;
        }
        else
        {
            transform.parent = null;
        }
        return rayHit.collider != null || rayHitMovingPlatform.collider != null;
    }
}
