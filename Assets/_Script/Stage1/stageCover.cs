using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stageCover : MonoBehaviour
{
    SpriteRenderer spr;
    public float fadeSpeed;
    public bool isCoverReveal;
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
    }


    void Update()
    {
        if (isCoverReveal) //검은색 커버가 사라지게 만드는 코드
        {
            if(spr.color.a <= 0)
            {
                return; //투명도가 0 이면 더이상 건드릴 필요가 없음 
            }
            spr.color = new Color(0, 0, 0, spr.color.a - fadeSpeed*Time.deltaTime);
        }

        if (!isCoverReveal) //검은색 커버가 생기게 만드는 코드
        {
            if (spr.color.a >= 1)
            {
                return; //투명도가 1 이면 더이상 건드릴 필요가 없음 
            }
            spr.color = new Color(0, 0, 0, spr.color.a + fadeSpeed * Time.deltaTime);
        }
    }
}
