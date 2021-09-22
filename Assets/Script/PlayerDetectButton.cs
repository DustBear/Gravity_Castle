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
    Vector2 rotationAngle;

    void Awake() {
        render = GetComponent<SpriteRenderer>();
        fireLauncher = transform.GetChild(0).GetComponent<FireLauncher>();
        fireLauncherLinear = transform.GetChild(0).GetComponent<FireLauncherLinear>();
        switch (transform.eulerAngles.z) {
            case 0f:
                rotationAngle = new Vector2(0, -1);
                break;
            case 90f:
                rotationAngle = new Vector2(1, 0);
                break;
            case 180f:
                rotationAngle = new Vector2(0, 1);
                break;
            default:
                rotationAngle = new Vector2(-1, 0);
                break;
        }
    }

    void Update() {
        RaycastHit2D rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.7f, 0.1f), transform.eulerAngles.z, transform.up, 0.5f, 1 << 10);
        if (!isPressed && rayHit.collider != null && Physics2D.gravity.normalized == rotationAngle) {
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
