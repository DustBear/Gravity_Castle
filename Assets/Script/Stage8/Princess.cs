using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Princess : MonoBehaviour
{
    Transform player;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        transform.rotation = player.rotation;
    }
}
