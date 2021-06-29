using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    Player player;

    void Awake() {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    void FixedUpdate()
    {     
        // rotation
        transform.rotation = player.transform.rotation;
        
        // position
        Vector3 playerPos = player.transform.position;
        float playerAngle = player.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector3 cameraPos = new Vector3(playerPos.x - 3 * Mathf.Sin(playerAngle), playerPos.y + 3 * Mathf.Cos(playerAngle), -10);
        /*if (player.gravityDirection == Player.GravityDirection.down || player.gravityDirection == Player.GravityDirection.up) {
            if (cameraPos.x < -10) {
                cameraPos.x = -10;
            }
            else if (cameraPos.x > 21) {
                cameraPos.x = 21;
            }
            if (cameraPos.y < -10) {
                cameraPos.y = -10;
            }
            else if (cameraPos.y > 32) {
                cameraPos.y = 32;
            }
        }
        else {
            if (cameraPos.x < -20) {
                cameraPos.x = -20;
            }
            else if (cameraPos.x > 31) {
                cameraPos.x = 31;
            }
            if (cameraPos.y < 0) {
                cameraPos.y = 0;
            }
            else if (cameraPos.y > 22) {
                cameraPos.y = 22;
            }
        }*/
        transform.position = cameraPos;
        
    }
}
