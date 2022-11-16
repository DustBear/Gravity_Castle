using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popUpSpikeBox : MonoBehaviour
{
    public float popUpDelay; //���ð� ���ٰ� �ٽ� ��������� �ɸ��� �ð�
    public float spikeDelay; //���ð� ���Դٰ� ������� �ɸ��� �ð� 

    public float iniOffset; //���� Ȱ��ȭ�ǰ� ó�� ���ð� ������ �� �ɸ��� �ð�
    public Sprite[] spriteGroup; //[0]�� ���ð� �� ��������Ʈ, [3]�� ���ð� Ƣ��� ��������Ʈ 

    [SerializeField] float[] spikeOffsetGroup;

    SpriteRenderer spr;
    BoxCollider2D spikeColl;

    public bool useAudioSource = false;

    AudioSource sound;

    public AudioClip spike_in;
    public AudioClip spike_out;

    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        spikeColl = GetComponent<BoxCollider2D>();

        spr.sprite = spriteGroup[0];
        spikeColl.offset = new Vector2(0, spikeOffsetGroup[0]);

        StartCoroutine(spikeLoop());

        if (useAudioSource)
        {
            sound = GetComponent<AudioSource>();
        }
    }


    void Update()
    {

    }

    IEnumerator spikeLoop()
    {
        yield return new WaitForSeconds(iniOffset);
        //�ʱ� ������ ���� ���� ���� Ƣ��� 

        while (true)
        {
            yield return new WaitForSeconds(popUpDelay - 0.2f);

            if (useAudioSource)
            {
                sound.PlayOneShot(spike_out);
            }

            for (int index = 0; index <= 3; index++)
            {
                spr.sprite = spriteGroup[index];
                spikeColl.offset = new Vector2(0, spikeOffsetGroup[index]);
                yield return new WaitForSeconds(0.05f);
            }

            yield return new WaitForSeconds(spikeDelay-0.2f);

            if (useAudioSource)
            {
                sound.PlayOneShot(spike_in);
            }

            for (int index=3; index>=0; index--)
            {
                spr.sprite = spriteGroup[index];
                spikeColl.offset = new Vector2(0, spikeOffsetGroup[index]);
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
