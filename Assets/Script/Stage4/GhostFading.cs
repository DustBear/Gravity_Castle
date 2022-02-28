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
    BoxCollider2D collid;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        collid = GetComponent<BoxCollider2D>();
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
            collid.enabled = true;
            for (float f = 0; f <= maxTransparency; f += 0.1f)
            {
                Color color = sprite.color;
                color.a = f;
                sprite.color = color;
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(keepFadeInTime);

            // Fade out
            for (float f = maxTransparency; f >= 0; f -= 0.1f)
            {
                Color color = sprite.color;
                color.a = f;
                sprite.color = color;
                yield return new WaitForSeconds(0.05f);
            }
            collid.enabled = false;
            yield return new WaitForSeconds(keepFadeOutTime);
        }
    }

    void Update()
    {
        transform.rotation = player.transform.rotation;
    }
}