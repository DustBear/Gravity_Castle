using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Princess : MonoBehaviour
{
    [SerializeField] PlayerStage8 player;

    void Update()
    {
        transform.rotation = player.transform.rotation;
    }
}
