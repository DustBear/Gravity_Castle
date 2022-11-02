using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gasPipeAnim : MonoBehaviour
{
    public Sprite[] gasSprite;
    public float period; //�� �� ������ �����ϰ� ���� ������� �� �ʳ� �ɸ����� 
    public float offset; //�� ó�� ������ �����ϰ� ù ���� ������� �� �ʳ� �ɸ����� 

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
