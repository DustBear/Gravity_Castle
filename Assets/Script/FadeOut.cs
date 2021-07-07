using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeOut : MonoBehaviour
{
    GameManager gameManager;
    UnityEngine.UI.Image fade;
    public float alpha;
    float time;

    void Awake() {
        fade = GetComponent<Image>();
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    }

    void OnEnable() {
        fade.color = new Color(0, 0, 0, 0);
        alpha = 0;
        time = 0;
    }

    void Update() {
        time += Time.deltaTime;
        if (alpha < 1 && time >= 0.1f) {
            alpha += 0.05f;
            fade.color = new Color(0, 0, 0, alpha);
            time = 0;
        }
        else if (alpha >= 1) {
            SceneManager.LoadScene(gameManager.respawnScene[gameManager.curStage, gameManager.curState]);
        }
    }
}
