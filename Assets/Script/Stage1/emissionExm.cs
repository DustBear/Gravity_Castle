using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class emissionExm : MonoBehaviour
{
    SpriteRenderer spr;
    public int arrayNum;
    public Sprite[] sprArray = new Sprite[7];
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        spr.sprite = sprArray[arrayNum];
    }

    void Update()
    {
        
    }
}
