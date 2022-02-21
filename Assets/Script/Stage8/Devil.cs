using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devil : MonoBehaviour
{
    [SerializeField] PlayerStage8 player;
    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        StartCoroutine(Behaviour());
    }

    void Update()
    {
        transform.rotation = player.transform.rotation;
    }

    IEnumerator Behaviour()
    {
        while (player.transform.position.y < 0f)
        {
            yield return new WaitForSeconds(1f);
        }
        rigid.gravityScale = 2f;
        while (true)
        {
            yield return new WaitForSeconds(5f);
            Walk(-12f);
            Jump();
            yield return new WaitForSeconds(5f);
            Walk(12f);
            Jump();
            yield return new WaitForSeconds(2f);
            Walk(20f);
            yield return new WaitForSeconds(2f);
            Walk(-20f);
        }
    }

    void Walk(float vel)
    {
        Vector2 locVel = transform.InverseTransformDirection(rigid.velocity);
        locVel = new Vector2(vel, locVel.y);
        rigid.velocity = transform.TransformDirection(locVel);
    }

    void Jump()
    {
        rigid.AddForce((transform.up * 15f), ForceMode2D.Impulse);
    }
}
