using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popUpSpike_audioSource : MonoBehaviour
{
    AudioSource sound;

    public AudioClip spike_in;
    public AudioClip spike_out;

    public float spikeActiveTime; //���ð� Ƣ��� ä�� Ȱ��ȭ�� �ִ� �ð�
    public float spikeDelayTime; //���ð� ���� Ƣ�������� �ɸ��� �ð� 
    public float spikeOffset; //���� ���۵ǰ� ó�� ���ð� Ƣ����� �� ���� �ɸ��� �ð� 

    float initDistance; //�÷��̾ ���� ����� ���� ���� �� ���� �Ÿ�
    GameObject playerObj;
    bool isCorPlaying;

    private void Awake()
    {
        sound = GetComponent<AudioSource>();
    }
    void Start()
    {
        sound.volume = 0f;
        StartCoroutine(spikeSound());
    }

    void Update()
    {
        
    }

    IEnumerator spikeSound()
    {
        yield return new WaitForSeconds(spikeOffset);

        while (true)
        {
            sound.PlayOneShot(spike_out);

            yield return new WaitForSeconds(spikeActiveTime);
            sound.PlayOneShot(spike_in);

            yield return new WaitForSeconds(spikeDelayTime);
        }             
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            playerObj = collision.gameObject;
            initDistance = (playerObj.transform.position - transform.position).magnitude;

            StartCoroutine(volumeCheck());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            //�÷��̾ �ݶ��̴� ���� �ִµ� �Ҹ��� �� �� ~> ���ʿ� �ݶ��̴� ������ �����Ʊ� ���� 
            //���� �Ҹ��� ����� ��� �� 
            if (!isCorPlaying)
            {
                if(GetComponent<CircleCollider2D>() != null)
                {
                    initDistance = GetComponent<CircleCollider2D>().radius;
                }
                StartCoroutine(volumeCheck());
            }
        }
    }

    public float soundVarDistance = 2f;
    IEnumerator volumeCheck()
    {
        isCorPlaying = true;

        while (true)
        {
            
            float curDistance = (playerObj.transform.position - transform.position).magnitude;
            float Distance_subtract = initDistance - curDistance;

            if(Distance_subtract > soundVarDistance)
            {
                sound.volume = 1f;
            }
            else if(Distance_subtract < 0)
            {
                sound.volume = 0f;
                isCorPlaying = false;
                yield break;
            }
            else
            {
                sound.volume = Distance_subtract / soundVarDistance;
            }

            yield return null;
        }
    }
}