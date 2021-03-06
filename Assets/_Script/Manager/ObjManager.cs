using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 오브젝트 풀링
public class ObjManager : Singleton<ObjManager>
{
    [HideInInspector] public enum ObjType {arrow, cannon, fire, fireFalling};

    [SerializeField] GameObject arrow;
    [SerializeField] GameObject cannon;
    [SerializeField] GameObject fire;
    [SerializeField] GameObject fireFalling;
    [SerializeField] int arrowNum;
    [SerializeField] int cannonNum;    
    [SerializeField] int fireNum;
    [SerializeField] int fireFallingNum;
    Queue<GameObject> arrowQueue;
    Queue<GameObject> cannonQueue;
    Queue<GameObject> fireQueue;
    Queue<GameObject> fireFallingQueue;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        InitPool(arrowNum, ref arrow, ref arrowQueue);
        InitPool(cannonNum, ref cannon, ref cannonQueue);
        InitPool(fireNum, ref fire, ref fireQueue);
        InitPool(fireFallingNum, ref fireFalling, ref fireFallingQueue);
    }

    void InitPool(int objNum, ref GameObject obj, ref Queue<GameObject> queue)
    {
        queue = new Queue<GameObject>();
        for (int i = 0; i < objNum; i++)
        {
            GameObject newObj = Instantiate(obj);
            DontDestroyOnLoad(newObj);
            queue.Enqueue(newObj);
            newObj.SetActive(false);
        }
    }

    public GameObject GetObj(ObjType objType)
    {
        switch (objType)
        {
            case ObjType.arrow:
                return arrowQueue.Dequeue();
            case ObjType.cannon:
                return cannonQueue.Dequeue();
            case ObjType.fire:
                return fireQueue.Dequeue();
            case ObjType.fireFalling:
                return fireFallingQueue.Dequeue();
        }
        return null;
    }

    public void ReturnObj(ObjType objType, GameObject obj)
    {
        obj.SetActive(false);
        switch (objType)
        {
            case ObjType.arrow:
                arrowQueue.Enqueue(obj);
                break;
            case ObjType.cannon:
                cannonQueue.Enqueue(obj);
                break;
            case ObjType.fire:
                fireQueue.Enqueue(obj);
                break;
            case ObjType.fireFalling:
                fireFallingQueue.Enqueue(obj);
                break;
        }
    }
}
