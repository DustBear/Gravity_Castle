using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class buttonTextColor : MonoBehaviour
{
    public float normalColor;
    public float highlightedColor;
    public float pressedColor;
    public float selectedColor;

    Text thisText;
    void Awake()
    {
        thisText = GetComponentInChildren<Text>();
        thisText.color = new Color(1, 1, 1, normalColor/255);
    }

    public void textNormal()
    {
        thisText.color = new Color(1,1,1, normalColor/255);
    }

    public void textHighlighted()
    {
        thisText.color = new Color(1, 1, 1, highlightedColor / 255);
    }

    public void textPressed()
    {
        thisText.color = new Color(1, 1, 1, pressedColor / 255);
    }

    public void textSelected()
    {
        thisText.color = new Color(1, 1, 1, selectedColor / 255);
    }
}
