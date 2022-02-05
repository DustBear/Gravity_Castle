using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenu : MonoBehaviour
{

    public void OnClickContinue() {
        gameObject.SetActive(false);
    }

    public void OnClickSave() {

    }

    public void OnClickExit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
