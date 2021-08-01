using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFading : MonoBehaviour
{
    Player player;
    float time;
    public float startTime;
    GhostFadingState ghostState;
    public enum State {fadeIn, keepFadeIn, fadeOut, keepFadeOut};
    public State state;

    void Awake() {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        ghostState = GetComponent<GhostFadingState>();
        transform.rotation = player.transform.rotation;
    }

    void Update()
    {
        time += Time.deltaTime;
        // state
        if (time >= startTime && !ghostState.isActiveAndEnabled) {
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

        // rotation
        transform.rotation = player.transform.rotation;
    }
}