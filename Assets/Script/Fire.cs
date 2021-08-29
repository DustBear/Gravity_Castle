using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    ParticleSystem particle;
    Vector2 additionalForce;

    void Awake() {
        particle = GetComponent<ParticleSystem>();
    }

    void OnEnable() {
        additionalForce = Physics2D.gravity.normalized;
    }
    void Update()
    {
        var forceOverLifetime = particle.forceOverLifetime;
        forceOverLifetime.x = -(Physics2D.gravity.normalized.x + additionalForce.x) * 5f;
        forceOverLifetime.y = -(Physics2D.gravity.normalized.y + additionalForce.y) * 5f;
    }
}
