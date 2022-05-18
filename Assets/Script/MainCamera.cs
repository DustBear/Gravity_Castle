using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    Player player;
    Camera cam;

    // �ػ�
    [SerializeField] float resolutionX = 1920.0f;
    [SerializeField] float resolutionY = 1080.0f;

    // ���� ������ ��ǥ
    [SerializeField] float mapMinX;
    [SerializeField] float mapMaxX;
    [SerializeField] float mapMinY;
    [SerializeField] float mapMaxY;

    // ī�޶� ��鸲
    [HideInInspector] public float shakedX {get; set;}
    [HideInInspector] public float shakedY {get; set;}
    [HideInInspector] public float shakedZ {get; set;}

    float angle; // viewRect ����
    float diagonal; // viewRect �밢�� ������ ����

    void Awake() {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        cam = GetComponent<Camera>();
        angle = Mathf.Atan2(resolutionX, resolutionY);
        diagonal = Mathf.Sqrt(Mathf.Pow(cam.orthographicSize, 2) + Mathf.Pow(cam.orthographicSize / resolutionY * resolutionX, 2));
    }

    void Update()
    {     
        // ī�޶� rotation = �÷��̾� rotation
        transform.rotation = Quaternion.Euler(0f, 0f, player.transform.eulerAngles.z + shakedZ);
        
        // �÷��̾� z-rotation�� 0�϶� ī�޶� ��ġ = �÷��̾� ��ġ + (0, 3)
        // �÷��̾� z-rotation�� �ٲ� �� �ֱ⿡ �ﰢ�Լ� ���
        float playerRot = player.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 playerPos = player.transform.position;
        float cameraX = playerPos.x - 3 * Mathf.Sin(playerRot);
        float cameraY = playerPos.y + 3 * Mathf.Cos(playerRot);
        
        // viewRect�� ������ ��ǥ
        float upLeftX = cameraX + diagonal * Mathf.Sin(playerRot + angle);
        float upLeftY = cameraY + diagonal * Mathf.Cos(playerRot + angle);
        float downLeftX = cameraX + diagonal * Mathf.Sin(playerRot + Mathf.PI - angle);
        float downLeftY = cameraY + diagonal * Mathf.Cos(playerRot + Mathf.PI - angle);
        float downRightX = cameraX + diagonal * Mathf.Sin(playerRot + Mathf.PI + angle);
        float downRightY = cameraY + diagonal * Mathf.Cos(playerRot + Mathf.PI + angle);
        float upRightX = cameraX + diagonal * Mathf.Sin(playerRot + Mathf.PI * 2 - angle);
        float upRightY = cameraY + diagonal * Mathf.Cos(playerRot + Mathf.PI * 2 - angle);
        
        // viewRect�� ������ ��ǥ�� �� �ۿ� �ִٸ� �� ������ �����ؾ���
        // deltaX, deltaY : �󸶳� �����ؾ��ϴ��� 
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

        // ī�޶� ���� ��ġ
        transform.position = new Vector3((upLeftX + downLeftX + downRightX + upRightX) / 4 + deltaX - shakedX, (upLeftY + downLeftY + downRightY + upRightY) / 4  + deltaY - shakedY, -10f);
    }
}
