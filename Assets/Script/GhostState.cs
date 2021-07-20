using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostState : MonoBehaviour
{
    SpriteRenderer sprite;
    Ghost ghost;
    Ghost.State state;
    public float maxTransparency;
    public float keepTime;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        ghost = GetComponent<Ghost>();
    }

    void OnEnable() {
        state = ghost.state;
        if (state == Ghost.State.fadeIn) {
            StartCoroutine(FadeIn());
        }
        else if (state == Ghost.State.fadeOut) {
            StartCoroutine(FadeOut());
        }
        else {
            StartCoroutine(Keep());
        }
    }

    IEnumerator FadeIn() {
        for (float f = 0; f <= maxTransparency; f += 0.1f) {
            Color color = sprite.material.color;
            color.a = f;
            sprite.material.color = color;
            yield return new WaitForSeconds(0.1f);
        }
        this.enabled = false;
    }

    IEnumerator FadeOut() {
        for (float f = maxTransparency; f >= 0; f -= 0.1f) {
            Color color = sprite.material.color;
            color.a = f;
            sprite.material.color = color;
            yield return new WaitForSeconds(0.1f);
        }
        this.enabled = false;
    }

    IEnumerator Keep() {
        yield return new WaitForSeconds(keepTime);
        this.enabled = false;
    }
}
