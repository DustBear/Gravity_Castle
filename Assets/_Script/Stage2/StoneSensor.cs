using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneSensor : MonoBehaviour
{
    [SerializeField] Rigidbody2D stoneRigid;
    [SerializeField] float shakeRange;
    [SerializeField] float shakeDuration;
    bool isShaked;

    public Sprite[] leverSprite;
    AudioSource leverSound;

    bool isPlayerOn;
    SpriteRenderer spr;

    private void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        spr.sprite = leverSprite[0];
        leverSound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && isPlayerOn)
        {
            StartCoroutine("spriteAni"); //작동여부에 상관없이 레버 애니메이션은 작동

            if(!isShaked && GameManager.instance.gameData.curAchievementNum <= 8)
            {
                StartCoroutine(StartShake());
            }          
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            isPlayerOn = true;
        }
    }

    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isPlayerOn = false;
        }
    }

    IEnumerator StartShake() {
        isShaked = true;
        float stopShakingTime = Time.time + shakeDuration;
        Vector2 stonePos = stoneRigid.transform.position;
        while (Time.time < stopShakingTime)
        {
            stoneRigid.transform.position = stonePos + Vector2.right * shakeRange;
            shakeRange *= -1f;
            yield return null;
        }
        stoneRigid.transform.position = stonePos;
        stoneRigid.bodyType = RigidbodyType2D.Dynamic;
        stoneRigid.gravityScale = 3f;
    }

    IEnumerator spriteAni() //스프라이트 움직임으로 애니메이션 구현
    {
        leverSound.Play();
        spr.sprite = leverSprite[1];
        yield return new WaitForSeconds(0.4f);

        spr.sprite = leverSprite[0];
    }
}
