using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stage2_lanton_ani : MonoBehaviour
{
    [SerializeField] Sprite[] lantonSprites;
    SpriteRenderer spr;

    bool isCorWorking;
    bool isPlayerOn = false;

    GameObject playerObj = null;

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        
    }

    
    void Update()
    {
        if (isCorWorking) return;

        if(isPlayerOn && playerObj.GetComponent<Rigidbody2D>().velocity.magnitude > 0f)
        {
            StartCoroutine(lantonAnim());
        }
    }

    IEnumerator lantonAnim()
    {
        isCorWorking = true;
        for(int index=0; index<lantonSprites.Length; index++)
        {
            spr.sprite = lantonSprites[index];
            yield return new WaitForSeconds(0.04f);
        }
        spr.sprite = lantonSprites[0];
        isCorWorking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            isPlayerOn = true;
            playerObj = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isPlayerOn = false;
            playerObj = null;
        }
    }
}
