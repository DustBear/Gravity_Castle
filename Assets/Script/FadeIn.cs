using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    public UnityEngine.UI.Image fade;
    float fades = 1;
    float time;

    void Awake() {
        fade.color = new Color(0, 0, 0, 1);
    }
    void Update() {
        time += Time.deltaTime;
        if (fades > 0 && time >= 0.1f) {
            fades -= 0.05f;
            fade.color = new Color(0, 0, 0, fades);
            time = 0;
        }
        else if (fades <= 0) {
            time = 0;
        }
    }
}
