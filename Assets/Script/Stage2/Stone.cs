using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    Rigidbody2D rigid;
    bool isCollideCannonHome;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Restrict velocity
        if (rigid.bodyType == RigidbodyType2D.Dynamic)
        {
            rigid.velocity = new Vector2(Mathf.Clamp(rigid.velocity.x, -20f, 20f), Mathf.Clamp(rigid.velocity.y, -20f, 20f));
        }

        // Fix position
        if (GameManager.instance.curAchievementNum >= 17)
        {
            transform.position = new Vector2(-176f, 14f);
        }
        else if (isCollideCannonHome)
        {
            transform.position = new Vector2(-176f, transform.position.y);
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            rigid.bodyType = RigidbodyType2D.Static;
        }
        if (other.gameObject.tag == "Cannon")
        {
            rigid.gravityScale = 10;
        }
        if (other.gameObject.tag == "CannonHome")
        {
            isCollideCannonHome = true;
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            rigid.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}
