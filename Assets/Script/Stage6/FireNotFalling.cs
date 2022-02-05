using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireNotFalling : MonoBehaviour
{
    GameObject player;
    [SerializeField] ParticleSystem fireParticle;
    [SerializeField] ParticleSystem sparkParticle;
    [SerializeField] ParticleSystem smokeParticle;

    void OnEnable() {
        if (GameManager.instance.curAchievementNum >= 0)
        {
            player = GameObject.FindWithTag("Player");
        }
    }

    void Update()
    {
        transform.rotation = player.transform.rotation;
    }

    public void StopParticle()
    {
        fireParticle.Stop();
        sparkParticle.Stop();
        smokeParticle.Stop();
    }
}
