using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStage7 : Player
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
        if (other.gameObject.tag == "Spike" || other.gameObject.tag == "Projectile") {
            GameManager.instance.isDie = true;
            UIManager.instance.FadeOut();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
    }

    protected override void IsGrounded()
    {
        RaycastHit2D rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.6f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 3 | 1 << 16 | 1 << 18 | 1 << 19);
        isGrounded = rayHit.collider != null;
    }
}
