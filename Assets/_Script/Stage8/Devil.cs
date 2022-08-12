using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devil : MonoBehaviour
{
    /*
    [SerializeField] GameObject laser;
    [SerializeField] CameraShakeStage8 cameraShake;
    Rigidbody2D rigid;
    Player player;
    IEnumerator coroutine;
    bool isBehaviourStarted;
    bool isLaserStarted;
    bool isSecondBehaviourStarted;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        coroutine = Behaviour();
        StartCoroutine(StartLaunch());
    }

    IEnumerator StartLaunch()
    {
        while (GameManager.instance.gameData.curAchievementNum <= 30)
        {
            yield return new WaitForSeconds(3f);
        }
        if (GameManager.instance.gameData.curAchievementNum <= 32)
        {
            isLaserStarted = true;
        }
    }

    void Update()
    {
        transform.rotation = player.transform.rotation;
        if (!isSecondBehaviourStarted)
        {
            if (GameManager.instance.gameData.curAchievementNum == 33 && GameManager.instance.isCliffChecked && player.transform.position.x > -115f)
            {
                StopCoroutine(coroutine);
                rigid.gravityScale = 2f;
                StartCoroutine(SecondBehaviour());
                isSecondBehaviourStarted = true;
            }
            else
            {
                if (isBehaviourStarted && Player.curState == Player.States.ChangeGravityDir)
                {
                    isBehaviourStarted = false;
                    rigid.gravityScale = 0f;
                    rigid.velocity = Vector2.zero;
                    StopCoroutine(coroutine);
                }
                else if (!isBehaviourStarted && Player.curState != Player.States.ChangeGravityDir)
                {
                    isBehaviourStarted = true;
                    rigid.gravityScale = 2f;
                    StartCoroutine(Wait());
                }
            }
        }
    }

    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(3f);
        StartCoroutine(coroutine);
    }

    public IEnumerator Behaviour()
    {
        while (GameManager.instance.gameData.curAchievementNum != 33)
        {
            Walk();
            Jump();
            yield return new WaitForSeconds(3f);
            if (isLaserStarted)
            {
                Launch();
            }
            yield return new WaitForSeconds(3f);
        }
        rigid.gravityScale = 0f;
        
    }

    IEnumerator SecondBehaviour()
    {
        var wait = new WaitForSeconds(7f);
        SecondWalk(-8f);
        Jump();
        while (!PlayerIsGrounded())
        {
            yield return null;
        }
        cameraShake.StartShaking(-10f, 90f);
        yield return wait;

        SecondWalk(-8f);
        Jump();
        while (!PlayerIsGrounded())
        {
            yield return null;
        }
        cameraShake.StartShaking(-10f, 0f);
        yield return wait;

        SecondWalk(-8f);
        Jump();
        while (!PlayerIsGrounded())
        {
            yield return null;
        }
        cameraShake.StartShaking(-10f, 270f);
        yield return wait;

        SecondWalk(-8f);
        Jump();
        while (!PlayerIsGrounded())
        {
            yield return null;
        }
        cameraShake.StartShaking(10f, 0f);
        yield return wait;

        SecondWalk(8f);
        Jump();
        while (!PlayerIsGrounded())
        {
            yield return null;
        }
        cameraShake.StartShaking(10f, 90f);
        yield return wait;

        SecondWalk(-8f);
        Jump();
        while (!PlayerIsGrounded())
        {
            yield return null;
        }
        cameraShake.StartShaking(10f, 180f);
        yield return wait;

        SecondWalk(-8f);
        Jump();
        while (!PlayerIsGrounded())
        {
            yield return null;
        }
        cameraShake.StartShaking(-10f, 90f);
        yield return wait;

        SecondWalk(8f);
        Jump();
        while (!PlayerIsGrounded())
        {
            yield return null;
        }
        cameraShake.StartShaking(10f, 180f);
        yield return wait;

        SecondWalk(-8f);
        Jump();
        while (!PlayerIsGrounded())
        {
            yield return null;
        }
        cameraShake.StartShaking(-10f, 90f);
        yield return wait;

        SecondWalk(8f);
        Jump();
        while (!PlayerIsGrounded())
        {
            yield return null;
        }
        cameraShake.StartShaking(10f, 180f);
    }

    void Walk()
    {
        Vector2 locVel = transform.InverseTransformDirection(rigid.velocity);
        Vector2 gravity = Physics2D.gravity.normalized;
        float vel = 0f;
        if ((int)gravity.y == -1)
        {
            vel = transform.position.x < player.transform.position.x ? 10f : -10f;
        }
        else if ((int)gravity.y == 1)
        {
            vel = transform.position.x > player.transform.position.x ? 10f : -10f;
        }
        else if ((int)gravity.x == -1)
        {
            vel = transform.position.y > player.transform.position.y ? 10f : -10f;
        }
        else
        {
            vel = transform.position.y < player.transform.position.y ? 10f : -10f;
        }
        locVel = new Vector2(vel, locVel.y);
        rigid.velocity = transform.TransformDirection(locVel);
    }

    void SecondWalk(float vel)
    {
        Vector2 locVel = transform.InverseTransformDirection(rigid.velocity);
        locVel = new Vector2(vel, locVel.y);
        rigid.velocity = transform.TransformDirection(locVel);
    }

    void Jump()
    {
        rigid.AddForce(transform.up * 28f, ForceMode2D.Impulse);
    }

    void Launch()
    {
        laser.transform.position = transform.position;
        laser.SetActive(true);
    }

    bool IsGrounded()
    {
        RaycastHit2D rayHit = Physics2D.BoxCast(transform.position, new Vector2(6f, 0.1f), transform.eulerAngles.z, -transform.up, 6f, 1 << 3);
        return rayHit.collider != null;
    }

    bool PlayerIsGrounded()
    {
        RaycastHit2D rayHit = Physics2D.BoxCast(player.transform.position, new Vector2(0.6f, 0.1f), transform.eulerAngles.z, -transform.up, 0.8f, 1 << 3);
        return rayHit.collider != null;
    }
    */
}
