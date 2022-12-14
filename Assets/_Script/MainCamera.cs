using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    GameObject playerObj;
    Player player;
    Camera cam;
    [SerializeField] float offset; //ī�޶� �÷��̾��� Y��ǥ�κ��� �󸶳� ������ �ִ°�(X��ǥ�� �׻� �÷��̾�� ����)
    [SerializeField] float smoothTime; //ī�޶� �÷��̾��� ��ġ�� �����ϴ� �� �ð��� �󸶳� �ɸ� ���ΰ�
    Vector3 dampSpeed = Vector3.zero;

    // �ػ�
    [SerializeField] float resolutionX = 1920.0f;
    [SerializeField] float resolutionY = 1080.0f;

    // ���� ������ ��ǥ
    [SerializeField] float mapMinX;
    [SerializeField] float mapMaxX;
    [SerializeField] float mapMinY;
    [SerializeField] float mapMaxY;

    float angle; // viewRect ����
    float diagonal; // viewRect �밢�� ������ ����

    public bool isCameraLock; //Ư�� ���� �� ī�޶� �÷��̾ ������ �ʵ��� ���� 
    public AnimationCurve shakeCurve;

    public bool isLookDownWork;
    public float lookDownSide_distance; //lookDownSide()�� �۵����� �� �Ʒ������� �þ߰� �󸶳� �̵��ϴ���
    public float lookDownSIde_smoothTime; //ī�޶� ��ũ smooth ���� 
    public float lookDownSide_inputTime; //�÷��̾ ���� �̻� down Arrow �� ������ �־�� �۵��ϴ���
    [SerializeField] float lookDownSide_timer= 0f;
    bool isCameraOffsetUp; //true�̸� 3, false�̸� -3 

    //public bool shouldLookDownStop;
    //�÷��̾ �� �Ʒ��� �ٶ󺸴� ������ ������ ���� ���¿��� ���� ������ ���� �߻� 
    //���� ���� �� �������̴� �ڷ�ƾ ��� �ߴ��ϴ� �ܺ� ���� �ʿ� 

    void Awake() 
    {
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
        isCameraOffsetUp = true;
        offset = 3f;
    }

    private void OnEnable()
    {
        ImmediateMove();
        isCameraLock = false; //ī�޶� ���� Ȱ��ȭ�� �� lock Ǯ����� �� 
    }
    void FixedUpdate()
    {
        DampMove();
    }  

    void Update()
    {
        lookDownSide();
        //if (isCameraLock) return;

        // ī�޶� rotation = �÷��̾� rotation
        transform.rotation = Quaternion.Euler(0f, 0f, player.transform.eulerAngles.z);
    }

    public void ImmediateMove()  //��ǥ ��ġ�� ��ٷ� �̵���(�� �ٲ�, �÷��̾ �� �� ������ �����̵��� �� ���)
    {        
        transform.position = cameraPosCal();
    }
    public void DampMove() //Damp ����� �ɸ� ä�� ������(�Ϲ����� �����ӿ��� ���)
    {
        //if (isCameraLock) return;

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
        int shakePeriod = 3;

        float strength;
        float elapsedTime = 0f;
        int shakeIndex = 0;

        while (elapsedTime <= shakeDelay)
        {
            elapsedTime += Time.unscaledDeltaTime;
            strength = shakeSize * shakeCurve.Evaluate(elapsedTime / shakeDelay);

            if(shakeIndex%shakePeriod == 0)
            {
                transform.position = new Vector3(cameraPosCal().x + strength * Random.insideUnitCircle.x, cameraPosCal().y + strength * Random.insideUnitCircle.y, cameraPosCal().z);
                //ī�޶� ��鸮�� ���߿��� �÷��̾ ���� ������ �� �ֵ��� ����� ��ǥ�� �̿� 
            }

            shakeIndex++;
            yield return null;
        }

        transform.position = cameraPosCal();
        player.GetComponent<Player>().isCameraShake = false;
    }
    
    void lookDownSide()
    {
        if (isLookDownWork) return;
        //ī�޶� �̵��ϴ� ������ ��ǲ �ν� x 

        bool isLookDownReady = false;

        //�Ʒ��� �ش��ϴ� ���� �� �ϳ��� �÷��̾� ���¸� ������Ű�� true �� �ǰ� �ϳ��� ������ �״�� false �̴� 
        if (Player.curState == Player.States.Walk) isLookDownReady = true;
        else if (Player.curState == Player.States.Grab) isLookDownReady = true;
        else if (Player.curState == Player.States.MoveOnRope) isLookDownReady = true;
        else if (Player.curState == Player.States.SelectGravityDir) isLookDownReady = true;

        if(isLookDownReady && Input.GetKeyDown(KeyCode.LeftShift))            
        {
            //�÷��̾ ���� ������ ������Ű�鼭 SHIFT �� ������

            if (isCameraOffsetUp)
            {
                isCameraOffsetUp = false;
                StartCoroutine(lookDownSideCor(true));
            }
            else
            {
                isCameraOffsetUp = true;
                StartCoroutine(lookDownSideCor(false));
            }

            /*
            lookDownSide_timer += Time.deltaTime;
            if(lookDownSide_timer >= lookDownSide_inputTime) //�ʿ� ����ġ�� ������ 
            {
                StartCoroutine(lookDownSideCor());
            }
            */
        }
    }

    IEnumerator lookDownSideCor(bool moveDown) //�Ķ���� true �̸� �Ʒ��� ������, �Ķ���� false�̸� ���� �ö� 
    {
        isLookDownWork = true;

        //isCameraLock = true; //��� ī�޶� �÷��̾ ������ �ʵ��� ���� 
        //InputManager.instance.isInputBlocked = true; //ī�޶� �����̴µ����� �÷��̾� ������ ���� 

        //Vector3 lookDownAimPos = transform.position - player.transform.up * lookDownSide_distance;

        float aimOffset; //ī�޶� �̵����⿡ ���� ��ǥ offset ���� 
        if (moveDown)
        {
            offset = 3f;
            aimOffset = -3f;
        }
        else
        {
            offset = -3f;
            aimOffset = 3f;
        }

        while(Mathf.Abs(offset - aimOffset) <= 0.1f)
        {
            //ī�޶��� offset�� aimOffset���� �ε巴�� �̵� 
            offset = Mathf.Lerp(offset, aimOffset, Time.deltaTime);
            yield return null;
        }

        //�������� �����ؼ� �̵� ������ ��ġ ������Ŵ 
        offset = aimOffset;
        isLookDownWork = false;

        /*
        while (true)
        {
            transform.position = Vector3.SmoothDamp(transform.position, lookDownAimPos, ref dampSpeed, lookDownSIde_smoothTime);          
            if(Mathf.Abs((transform.position - lookDownAimPos).magnitude) <= 0.02f)
            {
                transform.position = lookDownAimPos; //ī�޶� ��ġ ���� 
                break;
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }
        
        while (Input.GetKey(KeyCode.LeftShift) && !(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            //���� ȭ��ǥ�� ������ �ʰ� ���ÿ� ���� ����Ʈ Ű�� ������ ���� �� �Ʒ��� ���� ���� ���� 
            yield return null;
        }

        Vector3 newAimPos = transform.position + player.transform.up * lookDownSide_distance;
        InputManager.instance.isInputBlocked = false;
        
        while (true)
        {
            transform.position = Vector3.SmoothDamp(transform.position, newAimPos, ref dampSpeed, lookDownSIde_smoothTime);
            
            //ī�޶� ����ġ�� 100% ���ƿ� �� inputlock �� Ǯ���� �� ����� ~> �����ð��� �ΰ� inputlock Ǯ���� 
            if(Mathf.Abs((transform.position - newAimPos).magnitude) <= 0.1f)
            {
                if (InputManager.instance.isInputBlocked)
                {
                    //InputManager.instance.isInputBlocked = false;
                }
            }
            if (Mathf.Abs((transform.position - newAimPos).magnitude) <= 0.02f)
            {
                transform.position = newAimPos; //ī�޶� ��ġ ���� 
                break;
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }
        */
        //isCameraLock = false;

        //lookDownSide_timer = 0;
    }


    public Vector3 cameraPosCal()
    {
        // �÷��̾� z-rotation�� 0�϶� ī�޶� ��ġ = �÷��̾� ��ġ + (0, offset)
        // �÷��̾� z-rotation�� �ٲ� �� �ֱ⿡ �ﰢ�Լ� ���
        float playerRot = player.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 playerPos = player.transform.position;

        //ī�޶��� �߽��� ��ġ
        float cameraX = playerPos.x - offset * Mathf.Sin(playerRot);
        float cameraY = playerPos.y + offset * Mathf.Cos(playerRot);

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

        // ī�޶� ���� ��ġ
        Vector3 aimPos = new Vector3((upLeftX + downLeftX + downRightX + upRightX) / 4 + deltaX, (upLeftY + downLeftY + downRightY + upRightY) / 4 + deltaY, -10f);
       
        return aimPos;
    }
}
