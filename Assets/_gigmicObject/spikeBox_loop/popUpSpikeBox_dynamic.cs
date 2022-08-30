using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popUpSpikeBox_dynamic : MonoBehaviour
{
    //popUpSpikeBox와 비슷한 기믹이지만 정해진 범위 내에서 중력의 영향을 받아 움직임 

    public float popUpDelay; //가시가 들어갔다가 다시 나오기까지 걸리는 시간
    public float spikeDelay; //가시가 나왔다가 들어가기까지 걸리는 시간 

    public float iniOffset; //씬이 활성화되고 처음 가시가 나오는 데 걸리는 시간
    public Sprite[] spriteGroup; //[0]은 가시가 들어간 스프라이트, [1]은 가시가 튀어나온 스프라이트 

    SpriteRenderer spr;
    public BoxCollider2D spikeColl;

    void Start()
    {
        spr = GetComponent<SpriteRenderer>();        
        spr.sprite = spriteGroup[0];

        StartCoroutine(spikeLoop());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator spikeLoop()
    {
        yield return new WaitForSeconds(iniOffset);
        spr.sprite = spriteGroup[1]; //초기 오프셋 지난 이후 가시 튀어나옴 
        spikeColl.enabled = true; //가시 활성화 

        while (true)
        {
            yield return new WaitForSeconds(spikeDelay);
            spr.sprite = spriteGroup[0];
            spikeColl.enabled = false;

            yield return new WaitForSeconds(popUpDelay);
            spr.sprite = spriteGroup[1];
            spikeColl.enabled = true;
        }
    }
}
