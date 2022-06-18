using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stageCoverSensor : MonoBehaviour
{
    public GameObject fadeCover;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            StartCoroutine(makeFade());
        }
    }

    IEnumerator makeFade()
    {
        yield return new WaitForSeconds(1.5f);
        fadeCover.GetComponent<stageCover>().isCoverReveal = true; //플레이어가 콜라이더 통과하면 페이드 아웃
    }

}
