using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBox : MonoBehaviour
{
    GameManager gameManager;
    Animator boxBodyAnimator;
    Animator boxHeadAnimator;
    int keyBoxNum;

    void Awake() {
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        boxBodyAnimator = transform.GetChild(0).gameObject.GetComponent<Animator>();
        boxHeadAnimator = transform.GetChild(1).gameObject.GetComponent<Animator>();
        if (gameObject.CompareTag("KeyBox1")) {
            keyBoxNum = 1;
        }
        else {
            keyBoxNum = 2;
        }
    }

    void Update()
    {
        if (keyBoxNum == 1 && gameManager.isGetKey1 || keyBoxNum == 2 && gameManager.isGetKey2) {
            boxBodyAnimator.SetBool("getKey", true);
            boxHeadAnimator.SetBool("getKey", true);
        }
        else if (keyBoxNum == 1 && gameManager.isOpenKeyBox1 || keyBoxNum == 2 && gameManager.isOpenKeyBox2) {
            boxBodyAnimator.SetBool("allowKey", true);
            boxHeadAnimator.SetBool("allowKey", true);
            StartCoroutine("Wait");
        }
    }

    IEnumerator Wait() {
        yield return new WaitForSeconds(0.9f);
        // activate key
        transform.GetChild(2).gameObject.SetActive(true);
    }
}
