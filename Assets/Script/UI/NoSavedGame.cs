using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoSavedGame : MonoBehaviour
{
    // Ok버튼 클릭 -> 다시 메인메뉴로
    public void OnClickOk()
    {
        gameObject.SetActive(false);
    }
}
