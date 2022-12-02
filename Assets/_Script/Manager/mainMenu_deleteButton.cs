using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainMenu_deleteButton : MonoBehaviour
{
    public int selectedFileNum;
    //지울 파일 번호 

    public SaveFileButton[] saveFileButtonGroup;
    //4개의 세이브파일 버튼 그룹 

    public void onClick()
    {
        saveFileButtonGroup[selectedFileNum].saveDelete();
    }

}
