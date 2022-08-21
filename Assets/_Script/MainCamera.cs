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

    // ī�޶� ��鸲
    public float shakedX;
    public float shakedY;
    public float shakedZ;

    float angle; // viewRect ����
    float diagonal; // viewRect �밢�� ������ ����

    public bool isCameraLock; //Ư�� ���� �� ī�޶� �������� �ʵ��� ���� 
    public AnimationCurve shakeCurve;

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
        isCameraLock = false; //ī�޶� ���� Ȱ��ȭ�� �� lock Ǯ����� �� 
    }
    void FixedUpdate()
    {
        if (isCameraLock) return;
        DampMove();
    }  

    void Update()
    {
        if (isCameraLock) return;

        // ī�޶� rotation = �÷��̾� rotation
        transform.rotation = Quaternion.Euler(0f, 0f, player.transform.eulerAngles.z + shakedZ);
    }

    public void ImmediateMove()  //��ǥ ��ġ�� ��ٷ� �̵���(�� �ٲ�, �÷��̾ �� �� ������ �����̵��� �� ���)
    {        
        transform.position = cameraPosCal();
    }
    public void DampMove() //Damp ����� �ɸ� ä�� ������(�Ϲ����� �����ӿ��� ���)
    {               
        Vector3 aimPos = cameraPosCal();
        transform.position = Vector3.SmoothDamp(transform.position, aimPos, ref dampSpeed, smoothTime);
    }

    public void cameraShake(float shakeSize, float shakeCount)
    {
        StartCoroutine(cameraShakeCor(shakeSize, shakeCount));
    }
    IEnumerator cameraShakeCor(float shakeSize, float shakeDelay)
    {
        Vector2 initialPos = new Vector2(transform.position.x, transform.position.y);
        player.GetComponent<Player>().isCameraShake = true;

        float strength;
        float elapsedTime = 0f;

        while(elapsedTime <= shakeDelay)
        {
            elapsedTime += Time.deltaTime;
            strength = shakeSize * shakeCurve.Evaluate(elapsedTime / shakeDelay);

            transform.position = initialPos + strength * Random.insideUnitCircle;
            yield return null;
        }

        transform.position = initialPos;       
        player.GetComponent<Player>().isCameraShake = false;
    }

    Vector3 cameraPosCal()
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
        Vector3 aimPos = new Vector3((upLeftX + downLeftX + downRightX + upRightX) / 4 + deltaX - shakedX, (upLeftY + downLeftY + downRightY + upRightY) / 4 + deltaY - shakedY, -10f);
       
        return aimPos;
    }
}
