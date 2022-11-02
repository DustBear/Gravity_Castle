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

    float angle; // viewRect 각도
    float diagonal; // viewRect 대각선 길이의 절반

    public bool isCameraLock; //특정 연출 시 카메라가 플레이어를 따라가지 않도록 고정 
    public AnimationCurve shakeCurve;

    bool isLookDownWork;
    public float lookDownSide_distance; //lookDownSide()가 작동했을 때 아래쪽으로 시야가 얼마나 이동하는지
    public float lookDownSIde_smoothTime; //카메라 워크 smooth 정도 
    public float lookDownSide_inputTime; //플레이어가 몇초 이상 down Arrow 를 누르고 있어야 작동하는지
    [SerializeField] float lookDownSide_timer= 0f;

    void Awake() {
        playerObj = GameObject.FindWithTag("Player");
        player = playerObj.GetComponent<Player>();
        cam = GetComponent<Camera>();
        angle = Mathf.Atan2(resolutionX, resolutionY);
        diagonal = Mathf.Sqrt(Mathf.Pow(cam.orthographicSize, 2) + Mathf.Pow(cam.orthographicSize / resolutionY * resolutionX, 2));

        isCameraLock = false;
        isLookDownWork = false;
    }

    void Start()
    {
        ImmediateMove();
    }

    private void OnEnable()
    {
        ImmediateMove();
        isCameraLock = false; //카메라가 새로 활성화될 땐 lock 풀어줘야 함 
    }
    void FixedUpdate()
    {
        DampMove();
    }  

    void Update()
    {
        lookDownSide();
        if (isCameraLock) return;

        // 카메라 rotation = 플레이어 rotation
        transform.rotation = Quaternion.Euler(0f, 0f, player.transform.eulerAngles.z);
    }

    public void ImmediateMove()  //목표 위치로 곧바로 이동함(씬 바뀔때, 플레이어가 한 씬 내에서 순간이동할 때 사용)
    {        
        transform.position = cameraPosCal();
    }
    public void DampMove() //Damp 기능이 걸린 채로 움직임(일반적인 움직임에서 사용)
    {
        if (isCameraLock) return;

        Vector3 aimPos = cameraPosCal();
        transform.position = Vector3.SmoothDamp(transform.position, aimPos, ref dampSpeed, smoothTime);
    }

    public void cameraShake(float shakeSize, float shakeCount)
    {
        StartCoroutine(cameraShakeCor(shakeSize, shakeCount));
    }
    IEnumerator cameraShakeCor(float shakeSize, float shakeDelay)
    {
        Vector3 initialPos = transform.position;
        player.GetComponent<Player>().isCameraShake = true;

        float strength;
        float elapsedTime = 0f;

        while(elapsedTime <= shakeDelay)
        {
            elapsedTime += Time.unscaledDeltaTime;
            strength = shakeSize * shakeCurve.Evaluate(elapsedTime / shakeDelay);

            transform.position = new Vector3(cameraPosCal().x + strength * Random.insideUnitCircle.x, cameraPosCal().y + strength * Random.insideUnitCircle.y, cameraPosCal().z);
            //카메라가 흔들리는 도중에도 플레이어를 따라 움직일 수 있도록 상대적 좌표를 이용 
            yield return null;
        }

        transform.position = cameraPosCal();      
        player.GetComponent<Player>().isCameraShake = false;
    }
    
    void lookDownSide()
    {
        if (isLookDownWork) return;

        bool isLookDownReady = false;

        //아래에 해당하는 조건 중 하나라도 플레이어 상태를 만족시키면 true 가 되고 하나도 없으면 그대로 false 이다 
        if (Player.curState == Player.States.Walk) isLookDownReady = true;
        else if (Player.curState == Player.States.Grab) isLookDownReady = true;
        else if (Player.curState == Player.States.MoveOnRope) isLookDownReady = true;
        else if (Player.curState == Player.States.SelectGravityDir) isLookDownReady = true;

        if(isLookDownReady && Input.GetKey(KeyCode.LeftShift)) 
            //플레이어가 상태 조건을 만족시키면서 SHIFT 를 계속 누르면
        {
            lookDownSide_timer += Time.deltaTime;
            if(lookDownSide_timer >= lookDownSide_inputTime) //필요 기준치를 넘으면 
            {
                StartCoroutine(lookDownSideCor());
            }
        }
    }

    IEnumerator lookDownSideCor()
    {
        isLookDownWork = true;
        isCameraLock = true; //잠시 카메라가 플레이어를 따라가지 않도록 조정 
        InputManager.instance.isInputBlocked = true; //카메라 움직이는동안은 플레이어 움직임 정지 

        Vector3 lookDownAimPos = transform.position - player.transform.up * lookDownSide_distance;

        while (true)
        {
            transform.position = Vector3.SmoothDamp(transform.position, lookDownAimPos, ref dampSpeed, lookDownSIde_smoothTime);          
            if(Mathf.Abs((transform.position - lookDownAimPos).magnitude) <= 0.02f)
            {
                transform.position = lookDownAimPos; //카메라 위치 고정 
                break;
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }
        
        while (Input.GetKey(KeyCode.LeftShift) && !(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            yield return null;
        }

        Vector3 newAimPos = transform.position + player.transform.up * lookDownSide_distance;
        InputManager.instance.isInputBlocked = false;

        while (true)
        {
            transform.position = Vector3.SmoothDamp(transform.position, newAimPos, ref dampSpeed, lookDownSIde_smoothTime);

            //카메라가 원위치로 100% 돌아온 후 inputlock 이 풀리면 좀 답답함 ~> 여유시간을 두고 inputlock 풀어줌 
            if(Mathf.Abs((transform.position - newAimPos).magnitude) <= 0.1f)
            {
                if (InputManager.instance.isInputBlocked)
                {
                    //InputManager.instance.isInputBlocked = false;
                }
            }
            if (Mathf.Abs((transform.position - newAimPos).magnitude) <= 0.02f)
            {
                transform.position = newAimPos; //카메라 위치 고정 
                break;
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }

        isLookDownWork = false;
        isCameraLock = false;

        lookDownSide_timer = 0;
    }


    public Vector3 cameraPosCal()
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
        Vector3 aimPos = new Vector3((upLeftX + downLeftX + downRightX + upRightX) / 4 + deltaX, (upLeftY + downLeftY + downRightY + upRightY) / 4 + deltaY, -10f);
       
        return aimPos;
    }
}
