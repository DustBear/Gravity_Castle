using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLauncherLasting : MonoBehaviour
{
    [SerializeField] GameObject fire;
    [SerializeField] bool isVertical;
    [SerializeField] Vector2 leftPos;
    [SerializeField] Vector2 rightPos;
    public Vector2 finalPos;
    [SerializeField] float curSpeed;
    [SerializeField] float deltaSpeed;
    
    void Start() {
        // Create fire
        if (!isVertical)
        {
            for (float f = leftPos.x; f <= rightPos.x; f += 0.5f)
            {
                GameObject newFire = Instantiate(fire, new Vector2(f, transform.position.y), Quaternion.identity);
                newFire.transform.parent = transform;
            }
        }
        else
        {
            for (float f = leftPos.y; f <= rightPos.y; f += 0.5f)
            {
                GameObject newFire = Instantiate(fire, new Vector2(transform.position.x, f), Quaternion.identity);
                newFire.transform.parent = transform;
            }
        }

        // Start to move
        IEnumerator coroutine = IncreaseSpeed();
        StartCoroutine(coroutine);
        StartCoroutine(Move(coroutine));
    }

    IEnumerator Move(IEnumerator increaseSpeed) {
        while (Vector2.Distance(transform.position, finalPos) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, finalPos, Time.deltaTime * curSpeed);
            yield return null;
        }
        StopCoroutine(increaseSpeed);
    }

    IEnumerator IncreaseSpeed()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            curSpeed += deltaSpeed;
        }
    }
}
