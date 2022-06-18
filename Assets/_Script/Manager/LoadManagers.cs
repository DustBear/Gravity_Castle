using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Editor 내의 어떤 씬에서 재생 버튼을 누르든 Managers 씬에서 시작
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
