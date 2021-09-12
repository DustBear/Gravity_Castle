using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectButton : MonoBehaviour
{
    SpriteRenderer render;
    FireLauncher fireLauncher;
    FireLauncherLinear fireLauncherLinear;
    bool isGreen;
    bool isPressed;

    void Awake() {
        render = GetComponent<SpriteRenderer>();
        fireLauncher = transform.GetChild(0).GetComponent<FireLauncher>();
        fireLauncherLinear = transform.GetChild(0).GetComponent<FireLauncherLinear>();
    }

    void Update() {
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, transform.up, 0.5f, 1 << 10);
        if (!isPressed && rayHit.collider != null && rayHit.collider.CompareTag("Player")) {
            render.color = new Color(1 - render.color.r, 1 - render.color.g, 0f, 1f);
            isGreen = !isGreen;
            fireLauncher.enabled = isGreen;
            fireLauncherLinear.enabled = !isGreen;
            isPressed = true;
        }
        if (rayHit.collider == null) {
            isPressed = false;
        }
    }
}
