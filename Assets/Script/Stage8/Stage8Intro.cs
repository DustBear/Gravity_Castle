using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage8Intro : MonoBehaviour
{
    // [SerializeField] PlayerStage8 player;
    // [SerializeField] CameraShakeStage8 cameraShake;

    // void Start()
    // {
    //     StartCoroutine(Shaking());
    // }

    // IEnumerator Shaking()
    // {
    //     // First shake
    //     while (player.transform.position.x < -76f)
    //     {
    //         if (player.transform.position.x > -97f)
    //         {
    //             cameraShake.shakeDir = -2f;
    //             cameraShake.enabled = true;
    //         }
    //         else
    //         {
    //             cameraShake.enabled = false;
    //         }
    //         yield return new WaitForSeconds(0.1f);
    //     }
    //     cameraShake.enabled = false;
    //     player.devilLeveringRotation = 270f;
    //     player.isDevilRotating = true;
    //     while (player.isDevilRotating)
    //     {
    //         yield return new WaitForSeconds(0.1f);
    //     }

    //     // Second shake
    //     cameraShake.shakeDir = -2f;
    //     cameraShake.enabled = true;
    //     while (player.transform.position.y < -4f)
    //     {
    //         yield return new WaitForSeconds(0.1f);
    //     }
    //     cameraShake.enabled = false;
    //     player.devilLeveringRotation = 180f;
    //     player.isDevilRotating = true;
    // }
}
