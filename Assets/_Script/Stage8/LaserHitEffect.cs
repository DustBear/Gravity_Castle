using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserHitEffect : MonoBehaviour
{
    public void StopHitEffect()
    {
        gameObject.SetActive(false);
    }
}
