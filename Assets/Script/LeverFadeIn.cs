using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverFadeIn : MonoBehaviour
{
    SpriteRenderer sprite;

    void Awake() {
        sprite = GetComponent<SpriteRenderer>();
    }
    void Start() {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn() {
        for (float alpha = 0f; alpha <= 1f; alpha += 0.1f) {
            Color color = sprite.color;
            color.a = alpha;
            sprite.color = color;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
