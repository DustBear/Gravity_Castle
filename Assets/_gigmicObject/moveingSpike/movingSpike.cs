using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingSpike : MonoBehaviour
{
    public Vector2 pos1, pos2; //pos1�� pos2 ���� ���� ŭ 
    public float moveDelay; //spike�� pos1 �� pos2 ���̸� ������ �� �ɸ��� �ð� 
    public float stopDelay; //spike�� �� �������� �ӹ��� �ִ� �ð� 
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
        //�ʱ� �ӵ� 
        float moveSpeed = (pos1 - pos2).magnitude / moveDelay;

        if(initPos == 1) //pos1���� ��� ~> pos2�� ���� �� 
        {
            rigid.velocity = (pos2 - pos1).normalized * moveSpeed;
            anim.SetFloat("animSpeed", -1);
        }
        else //pos2���� ��� ~> pos1�� ���� �� 
        {
            rigid.velocity = (pos1 - pos2).normalized * moveSpeed;
            anim.SetFloat("animSpeed", 1);
        }

        while (true)
        {
            Vector2 spikePos = new Vector2(transform.position.x, transform.position.y);
            if ((spikePos - pos1).magnitude > (pos2-pos1).magnitude) //spikeBox �� pos2�� �Ѿ 
            {
                transform.position = pos2;
                rigid.velocity = Vector2.zero;
                anim.SetFloat("animSpeed", 0);
                yield return new WaitForSeconds(stopDelay);

                rigid.velocity = rigid.velocity = (pos1 - pos2).normalized * moveSpeed; //pos1 �� ���� ��� 
                anim.SetFloat("animSpeed", 1);
            }
            else if((spikePos - pos2).magnitude > (pos2 - pos1).magnitude) //spikeBox �� pos1�� �Ѿ 
            {
                transform.position = pos1;
                rigid.velocity = Vector2.zero;
                anim.SetFloat("animSpeed", 0);
                yield return new WaitForSeconds(stopDelay);

                rigid.velocity = rigid.velocity = (pos2 - pos1).normalized * moveSpeed; //pos2 �� ���� ��� 
                anim.SetFloat("animSpeed", -1);
            }
            yield return null;
        }
    }
}
