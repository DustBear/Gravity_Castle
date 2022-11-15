using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popUpSpike_audioSource : MonoBehaviour
{
    AudioSource sound;

    public AudioClip spike_in;
    public AudioClip spike_out;

    public float spikeActiveTime; //가시가 튀어나온 채로 활성화돼 있는 시간
    public float spikeDelayTime; //가시가 들어갔다 튀어나오기까지 걸리는 시간 
    public float spikeOffset; //씬이 시작되고 처음 가시가 튀어나오는 데 까지 걸리는 시간 

    float initDistance; //플레이어가 사운드 영향권 내에 들어갔을 때 최초 거리
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
            //플레이어가 콜라이더 내에 있는데 소리가 안 남 ~> 애초에 콜라이더 내에서 생성됐기 때문 
            //따로 소리를 재생해 줘야 함 
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