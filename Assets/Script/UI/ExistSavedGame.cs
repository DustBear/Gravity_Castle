using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExistSavedGame : MonoBehaviour
{
    // Start New game
    public void OnClickYes()
    {
        GameManager.instance.StartGame(true);
        gameObject.SetActive(false);
    }

    // Back to the main menu
    public void OnClickNo()
    {
        gameObject.SetActive(false);
    }
}
