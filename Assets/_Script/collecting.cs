using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collecting : MonoBehaviour
{
    GameObject cameraObj;
    MainCamera cameraScript;
    public ParticleSystem part;

    bool isParticlePlayed;

    public AudioSource sound;
    public AudioSource loopSound;

    public AudioClip collect_normal;
    public AudioClip collect_abnormal;
    public AudioClip ambience;

    private void Awake()
    {

    }
    void Start()
    {
        cameraObj = GameObject.FindWithTag("MainCamera");
        cameraScript = cameraObj.GetComponent<MainCamera>();

        isParticlePlayed = false;
        loopSound.clip = ambience;
        loopSound.Play();
    }

    
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {            
            if(collision.transform.up == transform.up)
            {
                if (!isParticlePlayed)
                {
                    part.Play();
                    GetComponent<AudioSource>().Play();
                    sound.PlayOneShot(collect_normal);

                    UIManager.instance.cameraShake(0.5f, 0.3f);
                    GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0); //≈ı∏Ì»≠ 

                    isParticlePlayed = true;
                }

                Invoke("deActive", 2f);
            }
            else
            {
                sound.PlayOneShot(collect_abnormal);
            }
        }
    }

    void deActive()
    {
        gameObject.SetActive(false);
    }
}
