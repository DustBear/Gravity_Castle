using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    // move
    public enum MoveType {linear, ellipse};
    public MoveType moveType;
    public float speed;

    // move(linear)
    public Vector2 startPos, endPos;
    
    // move(ellipse)
    public float xRad, yRad;
    public float angle;
    Vector2 centerPos;
    float time;

    // state
    public bool useFade;
    GhostState ghostState;
    public enum State {fadeIn, keepFadeIn, fadeOut, keepFadeOut};
    public State state;

    void Awake() {
        ghostState = GetComponent<GhostState>();
        centerPos = transform.position;
    }

    void Start() {
        angle *= Mathf.Deg2Rad;
    }

    void Update()
    {
        time += Time.deltaTime;
        // move (linear)
        if (moveType == MoveType.linear) {
            transform.position = new Vector2(startPos.x + (endPos.x - startPos.x) * (Mathf.Sin(time * speed) / 2.0f + 0.5f)
            , startPos.y + (endPos.y - startPos.y) * (Mathf.Sin(time * speed) / 2.0f + 0.5f));
        }
        // move (ellipse)
        else if (moveType == MoveType.ellipse) {
            transform.position = centerPos + new Vector2(xRad * Mathf.Sin(angle + time * speed), yRad * Mathf.Cos(angle + time * speed));
        }

        // state
        if (useFade && !ghostState.isActiveAndEnabled) {
            if (state == State.fadeIn) {
                state = State.keepFadeIn;
            }
            else if (state == State.keepFadeIn) {
                state = State.fadeOut;
            }
            else if (state == State.fadeOut) {
                state = State.keepFadeOut;
            }
            else if (state == State.keepFadeOut) {
                state = State.fadeIn;
            }
            ghostState.enabled = true;
        }
    }
}
