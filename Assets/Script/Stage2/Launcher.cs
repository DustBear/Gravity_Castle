using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField] ObjManager.ObjType type;
    [SerializeField] float timeInterval;
    [SerializeField] float nextTime;
    [SerializeField] float firePower;
    bool isGravityChanged;
    int gravityChangedCount;

    void Start()
    {
        StartCoroutine(Launch());
    }

    IEnumerator Launch()
    {
        yield return new WaitForSeconds(nextTime);
        while (true)
        {
            while (gravityChangedCount > 0)
            {
                gravityChangedCount--;
                yield return new WaitForSeconds(0.64f); // rotation time
            }
            GameObject curObj = ObjManager.instance.GetObj(type);
            curObj.transform.position = transform.position;
            curObj.transform.eulerAngles = transform.eulerAngles;
            Projectile projectile = curObj.GetComponent<Projectile>();
            projectile.firePower = firePower;
            projectile.fireDir = transform.up;
            curObj.SetActive(true);
            yield return new WaitForSeconds(timeInterval);
        }
    }

    void Update() {
        if (!isGravityChanged && GameManager.instance.isChangeGravityDir)
        {
            isGravityChanged = true;
            gravityChangedCount++;
        }
        else if (isGravityChanged && !GameManager.instance.isChangeGravityDir)
        {
            isGravityChanged = false;
        }
    }
}
