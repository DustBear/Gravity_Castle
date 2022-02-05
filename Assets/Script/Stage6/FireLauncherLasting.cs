using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLauncherLasting : MonoBehaviour
{
    [SerializeField] bool isVertical;
    [SerializeField] Vector2 leftPos;
    [SerializeField] Vector2 rightPos;
    [SerializeField] Vector2 finalPos;
    [SerializeField] float curSpeed;
    [SerializeField] float deltaSpeed;
    
    void Start() {
        // Create fire
        for (float f = leftPos.y; f <= rightPos.y; f += 0.5f)
        {
            GameObject newFire = ObjManager.instance.GetObj(ObjManager.ObjType.fire);
            if (!isVertical)
            {
                newFire.transform.position = new Vector2(f, transform.position.y);
            }
            else
            {
                newFire.transform.position = new Vector2(transform.position.x, f);
            }
            newFire.transform.parent = gameObject.transform;
            newFire.SetActive(true);
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
