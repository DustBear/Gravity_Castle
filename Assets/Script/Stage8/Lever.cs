using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    SpriteRenderer render;

    void Awake()
    {
        render = GetComponent<SpriteRenderer>();
    }
    void OnEnable()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        while (render.color.a < 1f)
        {
            Color color = render.color;
            color.a += 0.1f;
            render.color = color;
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(3f);
        while (render.color.a > 0f)
        {
            Color color = render.color;
            color.a -= 0.1f;
            render.color = color;
            yield return new WaitForSeconds(0.1f);
        }
        gameObject.SetActive(false);
    }
}
