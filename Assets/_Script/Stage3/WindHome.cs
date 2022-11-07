using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindHome : MonoBehaviour
{
    [SerializeField] ParticleSystem windParticle;

    public Sprite[] spriteGroup;
    public GameObject particleKill;

    SpriteRenderer windHomeSpr;
    SpriteRenderer particleKillSpr;

    float defaulFrameDelay = 0.06f; //windForce가 60일 때의 애니메이션 회전 속도 

    IEnumerator windCor;
    void Awake() 
    {
        windHomeSpr = GetComponent<SpriteRenderer>();
        particleKillSpr = particleKill.GetComponent<SpriteRenderer>();

        windCor = windActive();
    }
    
    public void windAnimAct()
    {       
        StartCoroutine(windCor);
        windParticle.Play();
    }
    public void windAnimStop()
    {
        StopCoroutine(windCor);
        windParticle.Stop();
    }


    IEnumerator windActive()
    {
        var frameDelay = new WaitForSeconds(defaulFrameDelay);

        while (true)
        {
            for (int index = 0; index < spriteGroup.Length; index++)
            {              
                windHomeSpr.sprite = spriteGroup[index];
                particleKillSpr.sprite = spriteGroup[index];
                yield return frameDelay;
            }
        }       
    }
}
