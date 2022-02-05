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
        if (isCollideCannonHome)
        {
            transform.position = new Vector2(-175.6f, transform.position.y);
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
