using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    UnityEngine.UI.Image fade;
    float alpha;
    float time;

    void Awake() {
        fade = GetComponent<Image>();
    }

    void OnEnable() {
        fade.color = new Color(0, 0, 0, 1);
        alpha = 1;
    }

    void Update() {
        time += Time.deltaTime;
        if (alpha > 0 && time >= 0.1f) {
            alpha -= 0.05f;
            fade.color = new Color(0, 0, 0, alpha);
            time = 0;
        }
        else if (alpha <= 0) {
            time = 0;
        }
    }
}
