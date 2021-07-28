using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFadingState : MonoBehaviour
{
    SpriteRenderer sprite;
    GhostFading ghost;
    GhostFading.State state;
    public float maxTransparency;
    public float keepFadeInTime;
    public float keepFadeOutTime;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        Color color = sprite.material.color;
        color.a = maxTransparency;
        sprite.material.color = color;
        ghost = GetComponent<GhostFading>();
    }

    void OnEnable() {
        state = ghost.state;
        if (state == GhostFading.State.fadeIn) {
            StartCoroutine(FadeIn());
        }
        else if (state == GhostFading.State.keepFadeIn) {
            StartCoroutine(KeepFadeIn());
        }
        else if (state == GhostFading.State.fadeOut) {
            StartCoroutine(FadeOut());
        }
        else {
            StartCoroutine(KeepFadeOut());
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

    IEnumerator KeepFadeIn() {
        yield return new WaitForSeconds(keepFadeInTime);
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

    IEnumerator KeepFadeOut() {
        yield return new WaitForSeconds(keepFadeOutTime);
        this.enabled = false;
    }
}
