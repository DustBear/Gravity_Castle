using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoSavedGame : MonoBehaviour
{
    // Ok��ư Ŭ�� -> �ٽ� ���θ޴���
    public void OnClickOk()
    {
        gameObject.SetActive(false);
    }
}
