using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openingAnim_cut : MonoBehaviour
{
    public Sprite[] spriteGroup;
    public float frameDelay; //한 프레임 간격

    AudioSource sound;

    private void Awake()
    {
        sound = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        sound.Play();
    }
}
