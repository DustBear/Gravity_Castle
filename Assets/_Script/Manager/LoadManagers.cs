using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Editor ���� � ������ ��� ��ư�� ������ Managers ������ ����
public class LoadManagers : MonoBehaviour
{
    const string ManagerSceneName = "Managers";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void FirstLoad()
    {
        if (SceneManager.GetActiveScene().name != ManagerSceneName)
        {
            SceneManager.LoadScene(ManagerSceneName);
        }
    }
}
