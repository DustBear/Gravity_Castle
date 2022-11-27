using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectionManager : MonoBehaviour
{
    [SerializeField] int collectionNum;
    [SerializeField] GameObject first_collection; //이 씬 내의 수집요소 중 가장 첫 번째

    InGameMenu ingameMenuScr;

    private void Awake()
    {
       
    }
    void Start()
    {
        ingameMenuScr = UIManager.instance.transform.Find("InGameMenu").GetComponent<InGameMenu>();
        ingameMenuScr.collectionCount = collectionNum;
        ingameMenuScr.firstCol_num = first_collection.GetComponent<collecting>().colNumCal;
    }

    void Update()
    {
        
    }
}
