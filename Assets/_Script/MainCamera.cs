using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    GameObject playerObj;
    Player player;
    Camera cam;
    [SerializeField] float offset; //카메라가 플레이어의 Y좌표로부터 얼마나 떨어져 있는가(X좌표는 항상 플레이어와 같다)
    [SerializeField] float smoothTime; //카메라가 플레이어의 위치에 도달하는 데 시간이 얼마나 걸릴 것인가
    Vector3 dampSpeed = Vector3.zero; 

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
        playerObj = GameObject.FindWithTag("Player");
        player = playerObj.GetComponent<Player>();
        cam = GetComponent<Camera>();
        angle = Mathf.Atan2(resolutionX, resolutionY);
        diagonal = Mathf.Sqrt(Mathf.Pow(cam.orthographicSize, 2) + Mathf.Pow(cam.orthographicSize / resolutionY * resolutionX, 2));
    }

    void Start()
    {
        ImmediateMove();
    }

    private void OnEnable()
    {
        ImmediateMove();
    }
    void FixedUpdate()
    {
        DampMove();
    }  

    public void ImmediateMove()  //목표 위치로 곧바로 이동함(씬 바뀔때, 플레이어가 한 씬 내에서 순간이동할 때 사용)
    {
        // 카메라 rotation = 플레이어 rotation
        transform.rotation = Quaternion.Euler(0f, 0f, player.transform.eulerAngles.z + shakedZ);

        transform.position = cameraPosCal();
    }
    public void DampMove() //Damp 기능이 걸린 채로 움직임(일반적인 움직임에서 사용)
    {
        
        // 카메라 rotation = 플레이어 rotation
        transform.rotation = Quaternion.Euler(0f, 0f, player.transform.eulerAngles.z + shakedZ);

        Vector3 aimPos = cameraPosCal();
        transform.position = Vector3.SmoothDamp(transform.position, aimPos, ref dampSpeed, smoothTime);
    }

    Vector3 cameraPosCal()
    {
        // 플레이어 z-rotation이 0일때 카메라 위치 = 플레이어 위치 + (0, offset)
        // 플레이어 z-rotation이 바뀔 수 있기에 삼각함수 사용
        float playerRot = player.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 playerPos = player.transform.position;

        //카메라의 중심점 위치
        float cameraX = playerPos.x - offset * Mathf.Sin(playerRot);
        float cameraY = playerPos.y + offset * Mathf.Cos(playerRot);

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
        if (cameraMinX < mapMinX)
        {
            deltaX = mapMinX - cameraMinX;
        }
        else if (cameraMaxX > mapMaxX)
        {
            deltaX = mapMaxX - cameraMaxX;
        }
        if (cameraMinY < mapMinY)
        {
            deltaY = mapMinY - cameraMinY;
        }
        else if (cameraMaxY > mapMaxY)
        {
            deltaY = mapMaxY - cameraMaxY;
        }

        // 카메라 최종 위치
        Vector3 aimPos = new Vector3((upLeftX + downLeftX + downRightX + upRightX) / 4 + deltaX - shakedX, (upLeftY + downLeftY + downRightY + upRightY) / 4 + deltaY - shakedY, -10f);
       
        return aimPos;
    }
}
