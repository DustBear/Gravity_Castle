using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFading : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] float startTime;
    [SerializeField] float maxTransparency;
    [SerializeField] float keepFadeInTime;
    [SerializeField] float keepFadeOutTime;
    SpriteRenderer sprite;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        Color color = sprite.color;
        color.a = maxTransparency;
        sprite.color = color;
    }

    void Start()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(startTime);
        while (true)
        {
            // Fade in
            for (float f = 0; f <= maxTransparency; f += 0.1f)
            {
                Color color = sprite.color;
                color.a = f;
                sprite.color = color;
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(keepFadeInTime);

            // Fade out
            for (float f = maxTransparency; f >= 0; f -= 0.1f)
            {
                Color color = sprite.color;
                color.a = f;
                sprite.color = color;
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(keepFadeOutTime);
        }
    }

    void Update()
    {
        transform.rotation = player.transform.rotation;
    }
}