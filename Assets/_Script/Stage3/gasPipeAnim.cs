using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gasPipeAnim : MonoBehaviour
{
    public Sprite[] gasSprite;
    public float period; //한 번 가스가 분출하고 다음 분출까지 몇 초나 걸리는지 
    public float offset; //맨 처음 게임을 시작하고 첫 가스 분출까지 몇 초나 걸리는지 

    float frameDelay=0.08f;
    SpriteRenderer spr;

    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        StartCoroutine(gasAnim());
    }

    IEnumerator gasAnim()
    {
        var frameTime = new WaitForSeconds(frameDelay);
        var periodTime = new WaitForSeconds(period);
        
        yield return new WaitForSeconds(offset);
        while (true)
        {
            for(int index=1; index<gasSprite.Length; index++)
            {
                spr.sprite = gasSprite[index];
                yield return frameTime;
            }
            spr.sprite = gasSprite[0];
            yield return periodTime;
        }
    }
}
