using static Define;
using System.Collections.Generic;
using UnityEngine;

public class UIWindow : MonoBehaviour
{
    public UIWindowType WindowType { get; private set; }
    public void Initialize(UIWindowType windowType)
    {
        WindowType = windowType;
        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}