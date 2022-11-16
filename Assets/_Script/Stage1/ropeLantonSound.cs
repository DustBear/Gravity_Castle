using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ropeLantonSound : MonoBehaviour
{
    AudioSource sound;

    public AudioClip ropeSound_1;
    public AudioClip ropeSound_2;

    private void Awake()
    {
        sound = GetComponent<AudioSource>();
    }

    void Start()
    {
        StartCoroutine(ropeSoundCor());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ropeSoundCor()
    {
        while (true)
        {
            sound.PlayOneShot(ropeSound_1);
            yield return new WaitForSeconds(1.18f);
            sound.PlayOneShot(ropeSound_2);
            yield return new WaitForSeconds(1.18f);
        }
    }
}
