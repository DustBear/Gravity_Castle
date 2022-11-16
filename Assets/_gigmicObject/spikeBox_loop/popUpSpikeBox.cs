using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popUpSpikeBox : MonoBehaviour
{
    public float popUpDelay; //가시가 들어갔다가 다시 나오기까지 걸리는 시간
    public float spikeDelay; //가시가 나왔다가 들어가기까지 걸리는 시간 

    public float iniOffset; //씬이 활성화되고 처음 가시가 나오는 데 걸리는 시간
    public Sprite[] spriteGroup; //[0]은 가시가 들어간 스프라이트, [3]은 가시가 튀어나온 스프라이트 

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
        //초기 오프셋 지난 이후 가시 튀어나옴 

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
