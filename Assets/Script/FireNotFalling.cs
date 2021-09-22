using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireNotFalling : MonoBehaviour
{
    Player player;
    ParticleSystem fireParticle, sparkParticle, smokeParticle;
    public bool isIceMelted;

    void Awake() {
        fireParticle = transform.GetChild(0).GetComponent<ParticleSystem>();
        sparkParticle = transform.GetChild(1).GetComponent<ParticleSystem>();
        smokeParticle = transform.GetChild(2).GetComponent<ParticleSystem>();
    }

    void OnEnable() {
        if (GameManager.instance.curStage != 0) {
            player = GameObject.FindWithTag("Player").GetComponent<Player>();
            isIceMelted = false;
        }
    }

    void Update()
    {
        transform.rotation = player.transform.rotation;
        
        if (isIceMelted) {
            fireParticle.Stop();
            sparkParticle.Stop();
            smokeParticle.Stop();
        }
    }
}
