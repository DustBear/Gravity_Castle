using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IcePlatform : MonoBehaviour
{
    [SerializeField] int iceNum;
    [SerializeField] bool isVertical;
    [SerializeField] Vector2 leftmostPos;
    [SerializeField] Vector2 rightmostPos;

    List<GameObject> fireList = new List<GameObject>();
    List<SpriteRenderer> leverRenderer;
    List<Vector2> leverPos;
    
    Tilemap tilemap;
    bool isFired;

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        if (transform.childCount == 0)
        {
            return;
        }
        leverRenderer = new List<SpriteRenderer>();
        leverPos = new List<Vector2>();
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject lever = transform.GetChild(i).gameObject;
            leverRenderer.Add(lever.GetComponent<SpriteRenderer>());
            leverPos.Add(lever.transform.position);
        }
    }

    void Start()
    {
        if (GameManager.instance.curIsMelted[iceNum])
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!isFired && other.gameObject.CompareTag("Projectile"))
        {
            GameManager.instance.curIsMelted[iceNum] = true;
            StartFired(other.transform.position);
            isFired = true;
        }
    }

    void StartFired(Vector2 startingPos)
    {
        // Set starting position of fire on the ice platform
        if (!isVertical)
        {
            float fracPart = startingPos.x - (int)startingPos.x;
            float intPart = startingPos.x - fracPart;
            if (fracPart >= 0f && fracPart < 0.25f)
            {
                startingPos = new Vector2(intPart, leftmostPos.y);
            }
            else if (fracPart >= 0.25f && fracPart < 0.75f)
            {
                startingPos = new Vector2(intPart + 0.5f, leftmostPos.y);
            }
            else
            {
                startingPos = new Vector2(intPart + 1.0f, leftmostPos.y);
            }

            if (startingPos.x < leftmostPos.x)
            {
                startingPos = new Vector2(leftmostPos.x, startingPos.y);
            }
            else if (startingPos.x > rightmostPos.x)
            {
                startingPos = new Vector2(rightmostPos.x, startingPos.y);
            }
        }
        else
        {
            float fracPart = startingPos.y - (int)startingPos.y;
            float intPart = startingPos.y - fracPart;
            if (fracPart >= 0f && fracPart < 0.25f)
            {
                startingPos = new Vector2(leftmostPos.x, intPart);
            }
            else if (fracPart >= 0.25f && fracPart < 0.75f)
            {
                startingPos = new Vector2(leftmostPos.x, intPart + 0.5f);
            }
            else
            {
                startingPos = new Vector2(leftmostPos.x, intPart + 1.0f);
            }

            if (startingPos.y < leftmostPos.y)
            {
                startingPos = new Vector2(startingPos.x, leftmostPos.y);
            }
            if (startingPos.y > rightmostPos.y)
            {
                startingPos = new Vector2(startingPos.x, rightmostPos.y);
            }
        }

        // Create fire on the starting position
        GameObject fire = ObjManager.instance.GetObj(ObjManager.ObjType.fire);
        fire.transform.position = startingPos;
        fire.SetActive(true);
        fireList.Add(fire);

        // Create fire on the lever
        for (int i = 0; i < transform.childCount; i++)
        {
            if (startingPos.x == leverPos[i].x || startingPos.y == leverPos[i].y)
            {
                fire = ObjManager.instance.GetObj(ObjManager.ObjType.fire);
                fire.transform.position = leverPos[i];
                fire.SetActive(true);
                fireList.Add(fire);
            }
        }

        // Create all remaining fire
        if (!isVertical)
        {
            StartCoroutine(CreateRemainingFire(startingPos, new Vector2(0.5f, 0f)));
        }
        else
        {
            StartCoroutine(CreateRemainingFire(startingPos, new Vector2(0f, 0.5f)));
        }
    }

    IEnumerator CreateRemainingFire(Vector2 startingPos, Vector2 deltaPos)
    {
        Vector2 leftPos = startingPos - deltaPos;
        Vector2 rightPos = startingPos + deltaPos;
        bool isFinishLeft = false;
        bool isFinishRight = false;
        while (!isFinishLeft || !isFinishRight)
        {
            yield return new WaitForSeconds(0.1f);
            if (Player.curState == Player.States.ChangeGravityDir)
            {
                continue;
            }

            // Check whether current position is end of ice platform
            if (!isVertical)
            {
                if (leftPos.x <= leftmostPos.x)
                {
                    isFinishLeft = true;
                }
                if (rightPos.x >= rightmostPos.x)
                {
                    isFinishRight = true;
                }
            }
            else
            {
                if (leftPos.y <= leftmostPos.y)
                {
                    isFinishLeft = true;
                }
                if (rightPos.y >= rightmostPos.y)
                {
                    isFinishRight = true;
                }
            }

            // If current position is not end of ice platform, create fire on this position
            if (!isFinishLeft)
            {
                leftPos -= deltaPos;
                CreateFire(leftPos);

            }
            if (!isFinishRight)
            {
                rightPos += deltaPos;
                CreateFire(rightPos);
            }
        }
        StartCoroutine(Melt());
    }

    void CreateFire(Vector2 firePos)
    {
        GameObject fire = ObjManager.instance.GetObj(ObjManager.ObjType.fire);
        fire.transform.position = firePos;
        fire.SetActive(true);
        fireList.Add(fire);
        for (int i = 0; i < transform.childCount; i++)
        {
            if (firePos.x != leverPos[i].x && firePos.y != leverPos[i].y)
            {
                continue;
            }
            fire = ObjManager.instance.GetObj(ObjManager.ObjType.fire);
            fire.transform.position = leverPos[i];
            fire.SetActive(true);
            fireList.Add(fire);
        }
    }

    IEnumerator Melt()
    {
        while (tilemap.color.a > 0f)
        {
            yield return new WaitForSeconds(0.1f);
            if (Player.curState == Player.States.ChangeGravityDir)
            {
                continue;
            }

            // Make tile and lever transparent
            Color tileColor = tilemap.color;
            if (tileColor.a > 0f)
            {
                tileColor.a -= 0.025f;
                tilemap.color = tileColor;
            }
            for (int i = 0; i < transform.childCount; i++)
            {
                Color leverColor = leverRenderer[i].color;
                leverColor.a -= 0.0625f;
                leverRenderer[i].color = leverColor;
            }
        }

        // Stop Particle
        for (int i = 0; i < fireList.Count; i++)
        {
            fireList[i].GetComponent<FireNotFalling>().StopParticle();
        }
        Destroy(gameObject, 1.0f); // 1.0f because we should wait particles to stop
    }

    void OnDestroy()
    {
        for (int i = 0; i < fireList.Count; i++)
        {
            if (fireList[i].activeSelf)
            {
                ObjManager.instance.ReturnObj(ObjManager.ObjType.fire, fireList[i]);
            }
        }
    }
}