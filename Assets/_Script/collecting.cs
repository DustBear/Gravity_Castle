using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collecting : MonoBehaviour
{
    GameObject cameraObj;
    MainCamera cameraScript;
    public ParticleSystem part;

    bool isParticlePlayed;

    private void Awake()
    {

    }
    void Start()
    {
        cameraObj = GameObject.FindWithTag("MainCamera");
        cameraScript = cameraObj.GetComponent<MainCamera>();

        isParticlePlayed = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && collision.transform.up == transform.up)
        {            
            if (!isParticlePlayed)
            {
                part.Play();               
                GetComponent<AudioSource>().Play();

                cameraScript.cameraShake(0.5f, 0.3f);
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0); //����ȭ 

                isParticlePlayed = true;
            }

            Invoke("deActive", 2f);
        }
    }

    void deActive()
    {
        gameObject.SetActive(false);
    }
}
