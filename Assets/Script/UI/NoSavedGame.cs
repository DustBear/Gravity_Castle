using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoSavedGame : MonoBehaviour
{
    public void OnClickOk()
    {
        gameObject.SetActive(false);
    }
}
