using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExistSavedGame : MonoBehaviour
{
    // Yes ��ư Ŭ�� -> �� ���� ����
    public void OnClickYes()
    {
        GameManager.instance.StartGame(true);
        gameObject.SetActive(false);
    }

    // No ��ư Ŭ�� -> �ٽ� ���θ޴���
    public void OnClickNo()
    {
        gameObject.SetActive(false);
    }
}
