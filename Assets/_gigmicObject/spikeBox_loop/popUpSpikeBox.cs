using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popUpSpikeBox : MonoBehaviour
{
    public float popUpDelay; //가시가 들어갔다가 다시 나오기까지 걸리는 시간
    public float spikeDelay; //가시가 나왔다가 들어가기까지 걸리는 시간 

    public float iniOffset; //씬이 활성화되고 처음 가시가 나오는 데 걸리는 시간
    public Sprite[] spriteGroup; //[0]은 가시가 들어간 스프라이트, [3]은 가시가 튀어나온 스프라이트 

    SpriteRenderer spr;
    BoxCollider2D spikeColl;

    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        spikeColl = GetComponent<BoxCollider2D>();
        spr.sprite = spriteGroup[0];

        StartCoroutine(spikeLoop());
    }


    void Update()
    {

    }

    IEnumerator spikeLoop()
    {
        yield return new WaitForSeconds(iniOffset);
        spr.sprite = spriteGroup[3]; //초기 오프셋 지난 이후 가시 튀어나옴 
        spikeColl.enabled = true; //가시 활성화 

        while (true)
        {
            yield return new WaitForSeconds(spikeDelay-0.2f);
            for(int index=3; index>=0; index--)
            {
                spr.sprite = spriteGroup[index];
                yield return new WaitForSeconds(0.05f);
            }
            spikeColl.enabled = false;

            yield return new WaitForSeconds(popUpDelay-0.2f);
            for (int index = 0; index <= 3; index++)
            {
                spr.sprite = spriteGroup[index];
                yield return new WaitForSeconds(0.05f);
            }
            spikeColl.enabled = true;
        }
    }
}
