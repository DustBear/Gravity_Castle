using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainMenu_deleteButton : MonoBehaviour
{
    public int selectedFileNum;
    //���� ���� ��ȣ 

    public SaveFileButton[] saveFileButtonGroup;
    //4���� ���̺����� ��ư �׷� 

    public void onClick()
    {
        saveFileButtonGroup[selectedFileNum].saveDelete();
    }

}
