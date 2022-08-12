using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popUpSpikeBox_dynamic : MonoBehaviour
{
    //popUpSpikeBox�� ����� ��������� ������ ���� ������ �߷��� ������ �޾� ������ 

    public float popUpDelay; //���ð� ���ٰ� �ٽ� ��������� �ɸ��� �ð�
    public float spikeDelay; //���ð� ���Դٰ� ������� �ɸ��� �ð� 

    public float iniOffset; //���� Ȱ��ȭ�ǰ� ó�� ���ð� ������ �� �ɸ��� �ð�
    public Sprite[] spriteGroup; //[0]�� ���ð� �� ��������Ʈ, [1]�� ���ð� Ƣ��� ��������Ʈ 

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
        spr.sprite = spriteGroup[1]; //�ʱ� ������ ���� ���� ���� Ƣ��� 
        spikeColl.enabled = true; //���� Ȱ��ȭ 

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
