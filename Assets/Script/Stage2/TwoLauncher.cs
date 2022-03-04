using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoLauncher : MonoBehaviour
{
    [SerializeField] ObjManager.ObjType type;
    [SerializeField] float timeInterval;
    [SerializeField] float nextTime;
    [SerializeField] float firePower;
    [SerializeField] Transform secondLauncher;
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
                yield return new WaitForSeconds(0.64f);
            }
            GameObject curObj1 = ObjManager.instance.GetObj(type);
            GameObject curObj2 = ObjManager.instance.GetObj(type);
            curObj1.transform.position = transform.position;
            curObj2.transform.position = secondLauncher.position;
            curObj1.transform.rotation = transform.rotation;
            curObj2.transform.rotation = secondLauncher.rotation;
            Projectile projectile1 = curObj1.GetComponent<Projectile>();
            Projectile projectile2 = curObj2.GetComponent<Projectile>();
            projectile1.type = type;
            projectile2.type = type;
            projectile1.firePower = firePower;
            projectile2.firePower = firePower;
            curObj1.SetActive(true);
            curObj2.SetActive(true);
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
