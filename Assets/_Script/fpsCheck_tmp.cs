using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fpsCheck_tmp : MonoBehaviour
{
    // ���� ���ߴܰ迡�� ������ üũ�� ���� GM �� �����ϴ� �ӽ� ��ũ��Ʈ 
    // ���� ���� �ܰ迡�� ��Ȱ��ȭ or �����ؾ� �� 

    [Range(1, 100)]
    public int fFont_size;

    [Range(0, 1)]
    public float Red, Green, Blue;

    float deltaTime = 0.0f;

    void Start()
    {
        fFont_size = fFont_size == 0 ? 50 : fFont_size;
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    private void OnGUI()
    {
        int w = Screen.width;
        int h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 0.02f);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / fFont_size;
        style.normal.textColor = new Color(Red, Green, Blue, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}