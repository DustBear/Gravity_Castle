using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireNotFalling : MonoBehaviour
{
    GameObject player;
    [SerializeField] ParticleSystem fireParticle;
    [SerializeField] ParticleSystem sparkParticle;
    [SerializeField] ParticleSystem smokeParticle;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable() {
        if (GameManager.instance.gameData.curAchievementNum >= 0)
        {
            player = GameObject.FindWithTag("Player");
        }
    }

    void Update()
    {
       // If scene changes, then erase all activated fires
        if (player != null)
        {
            transform.rotation = player.transform.rotation;
        }
    }

    public void StopParticle()
    {
        fireParticle.Stop();
        sparkParticle.Stop();
        smokeParticle.Stop();
    }
}
