using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectionManager : MonoBehaviour
{
    [SerializeField] int collectionNum;
    [SerializeField] GameObject first_collection; //�� �� ���� ������� �� ���� ù ��°

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
