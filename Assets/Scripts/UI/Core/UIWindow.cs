using static Define;
using System.Collections.Generic;
using UnityEngine;

public class UIWindow : WindowBase
{
    public bool isTestScene = false;
    public UIWindowType WindowType { get; private set; }
    public void Initialize(UIWindowType windowType)
    {
        WindowType = windowType;
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        Managers.Instance.isTestScene = isTestScene;
    }
}