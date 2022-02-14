using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    protected ObjManager() {}

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        arrowQueue = new Queue<GameObject>();
        cannonQueue = new Queue<GameObject>();
        fireQueue = new Queue<GameObject>();
        fireFallingQueue = new Queue<GameObject>();
        
        for (int i = 0; i < arrowNum; i++) {
            GameObject newObj = Instantiate(arrow);
            DontDestroyOnLoad(newObj);
            arrowQueue.Enqueue(newObj);
            newObj.SetActive(false);
        }
        for (int i = 0; i < cannonNum; i++) {
            GameObject newObj = Instantiate(cannon);
            DontDestroyOnLoad(newObj);
            cannonQueue.Enqueue(newObj);
            newObj.SetActive(false);
        }
        for (int i = 0; i < fireNum; i++) {
            GameObject newObj = Instantiate(fire);
            DontDestroyOnLoad(newObj);
            fireQueue.Enqueue(newObj);
            newObj.SetActive(false);
        }
        for (int i = 0; i < fireFallingNum; i++) {
            GameObject newObj = Instantiate(fireFalling);
            DontDestroyOnLoad(newObj);
            fireFallingQueue.Enqueue(newObj);
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
