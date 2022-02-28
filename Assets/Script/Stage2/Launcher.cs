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

    void Start()
    {
        StartCoroutine(Launch());
    }

    IEnumerator Launch()
    {
        yield return new WaitForSeconds(nextTime);
        while (true)
        {
            if (isGravityChanged)
            {
                yield return new WaitForSeconds(0.64f);
            }
            GameObject curObj = ObjManager.instance.GetObj(type);
            curObj.transform.position = transform.position;
            curObj.transform.rotation = transform.rotation;
            Projectile projectile = curObj.GetComponent<Projectile>();
            projectile.type = type;
            projectile.firePower = firePower;
            curObj.SetActive(true);
            yield return new WaitForSeconds(timeInterval);
        }
    }

    void Update() {
        if (!isGravityChanged && GameManager.instance.isChangeGravityDir)
        {
            isGravityChanged = true;
        }
        else if (isGravityChanged && !GameManager.instance.isChangeGravityDir)
        {
            isGravityChanged = false;
        }
    }
}
