using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    Player player;
    Camera cam;

    // 해상도
    [SerializeField] float resolutionX = 1920.0f;
    [SerializeField] float resolutionY = 1080.0f;

    // 맵의 꼭짓점 좌표
    [SerializeField] float mapMinX;
    [SerializeField] float mapMaxX;
    [SerializeField] float mapMinY;
    [SerializeField] float mapMaxY;

    // 카메라 흔들림
    [HideInInspector] public float shakedX {get; set;}
    [HideInInspector] public float shakedY {get; set;}
    [HideInInspector] public float shakedZ {get; set;}

    float angle; // viewRect 각도
    float diagonal; // viewRect 대각선 길이의 절반

    void Awake() {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        cam = GetComponent<Camera>();
        angle = Mathf.Atan2(resolutionX, resolutionY);
        diagonal = Mathf.Sqrt(Mathf.Pow(cam.orthographicSize, 2) + Mathf.Pow(cam.orthographicSize / resolutionY * resolutionX, 2));
    }

    void Update()
    {     
        // 카메라 rotation = 플레이어 rotation
        transform.rotation = Quaternion.Euler(0f, 0f, player.transform.eulerAngles.z + shakedZ);
        
        // 플레이어 z-rotation이 0일때 카메라 위치 = 플레이어 위치 + (0, 3)
        // 플레이어 z-rotation이 바뀔 수 있기에 삼각함수 사용
        float playerRot = player.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 playerPos = player.transform.position;
        float cameraX = playerPos.x - 3 * Mathf.Sin(playerRot);
        float cameraY = playerPos.y + 3 * Mathf.Cos(playerRot);
        
        // viewRect의 꼭짓점 좌표
        float upLeftX = cameraX + diagonal * Mathf.Sin(playerRot + angle);
        float upLeftY = cameraY + diagonal * Mathf.Cos(playerRot + angle);
        float downLeftX = cameraX + diagonal * Mathf.Sin(playerRot + Mathf.PI - angle);
        float downLeftY = cameraY + diagonal * Mathf.Cos(playerRot + Mathf.PI - angle);
        float downRightX = cameraX + diagonal * Mathf.Sin(playerRot + Mathf.PI + angle);
        float downRightY = cameraY + diagonal * Mathf.Cos(playerRot + Mathf.PI + angle);
        float upRightX = cameraX + diagonal * Mathf.Sin(playerRot + Mathf.PI * 2 - angle);
        float upRightY = cameraY + diagonal * Mathf.Cos(playerRot + Mathf.PI * 2 - angle);
        
        // viewRect의 꼭짓점 좌표가 맵 밖에 있다면 맵 안으로 조정해야함
        // deltaX, deltaY : 얼마나 조정해야하는지 
        float cameraMinX = Mathf.Min(downLeftX, downRightX, upLeftX, upRightX);
        float cameraMaxX = Mathf.Max(downLeftX, downRightX, upLeftX, upRightX);
        float cameraMinY = Mathf.Min(downLeftY, downRightY, upLeftY, upRightY);
        float cameraMaxY = Mathf.Max(downLeftY, downRightY, upLeftY, upRightY);

        float deltaX = 0;
        float deltaY = 0;
        if (cameraMinX < mapMinX) {
            deltaX = mapMinX - cameraMinX;
        }
        else if (cameraMaxX > mapMaxX) {
            deltaX = mapMaxX - cameraMaxX;
        }
        if (cameraMinY < mapMinY) {
            deltaY = mapMinY - cameraMinY;
        }
        else if (cameraMaxY > mapMaxY) {
            deltaY = mapMaxY - cameraMaxY;
        }

        // 카메라 최종 위치
        transform.position = new Vector3((upLeftX + downLeftX + downRightX + upRightX) / 4 + deltaX - shakedX, (upLeftY + downLeftY + downRightY + upRightY) / 4  + deltaY - shakedY, -10f);
    }
}
