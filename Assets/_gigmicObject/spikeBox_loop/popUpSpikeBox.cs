using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popUpSpikeBox : MonoBehaviour
{
    public float popUpDelay; //���ð� ���ٰ� �ٽ� ��������� �ɸ��� �ð�
    public float spikeDelay; //���ð� ���Դٰ� ������� �ɸ��� �ð� 

    public float iniOffset; //���� Ȱ��ȭ�ǰ� ó�� ���ð� ������ �� �ɸ��� �ð�
    public Sprite[] spriteGroup; //[0]�� ���ð� �� ��������Ʈ, [3]�� ���ð� Ƣ��� ��������Ʈ 

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
        spr.sprite = spriteGroup[3]; //�ʱ� ������ ���� ���� ���� Ƣ��� 
        spikeColl.enabled = true; //���� Ȱ��ȭ 

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
