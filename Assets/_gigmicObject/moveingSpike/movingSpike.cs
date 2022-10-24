using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingSpike : MonoBehaviour
{
    public Vector2 pos1, pos2; //pos1이 pos2 보다 값이 큼 
    public float moveDelay; //spike가 pos1 과 pos2 사이를 오가는 데 걸리는 시간 
    public float stopDelay; //spike가 양 끝점에서 머물러 있는 시간 
    public int initPos;

    Rigidbody2D rigid;
    Animator anim;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    void Start()
    {
        if (initPos == 1) 
        {
            transform.position = pos1;
        }
        else
        {
            transform.position = pos2;
        }

        anim.SetFloat("animSpeed", 0);
        StartCoroutine(spikeMove());
    }
   
    IEnumerator spikeMove()
    {
        //초기 속도 
        float moveSpeed = (pos1 - pos2).magnitude / moveDelay;

        if(initPos == 1) //pos1에서 출발 ~> pos2로 가야 함 
        {
            rigid.velocity = (pos2 - pos1).normalized * moveSpeed;
            anim.SetFloat("animSpeed", -1);
        }
        else //pos2에서 출발 ~> pos1로 가야 함 
        {
            rigid.velocity = (pos1 - pos2).normalized * moveSpeed;
            anim.SetFloat("animSpeed", 1);
        }

        while (true)
        {
            Vector2 spikePos = new Vector2(transform.position.x, transform.position.y);
            if ((spikePos - pos1).magnitude > (pos2-pos1).magnitude) //spikeBox 가 pos2를 넘어감 
            {
                transform.position = pos2;
                rigid.velocity = Vector2.zero;
                anim.SetFloat("animSpeed", 0);
                yield return new WaitForSeconds(stopDelay);

                rigid.velocity = rigid.velocity = (pos1 - pos2).normalized * moveSpeed; //pos1 을 향해 출발 
                anim.SetFloat("animSpeed", 1);
            }
            else if((spikePos - pos2).magnitude > (pos2 - pos1).magnitude) //spikeBox 가 pos1를 넘어감 
            {
                transform.position = pos1;
                rigid.velocity = Vector2.zero;
                anim.SetFloat("animSpeed", 0);
                yield return new WaitForSeconds(stopDelay);

                rigid.velocity = rigid.velocity = (pos2 - pos1).normalized * moveSpeed; //pos2 을 향해 출발 
                anim.SetFloat("animSpeed", -1);
            }
            yield return null;
        }
    }
}
