using UnityEngine;
using System.Collections.Generic;
using static Define;
public class UIManagerNew : SingletonMonoBehaviour<UIManagerNew>
{
    private Dictionary<UIWindowType, WindowBase> windowCache = new Dictionary<UIWindowType, WindowBase>();
    private UIStackManager<WindowBase> windowManager = new UIStackManager<WindowBase>();
    private UIStackManager<PopupBase> popupManager = new UIStackManager<PopupBase>();

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root;
        }
    }

    public Transform mainCanvas;
    public Transform MainCanvas
    {
        get
        {
            if (mainCanvas == null)
            {
                mainCanvas = Root.transform.GetComponent<Canvas>().transform;
            }
            return mainCanvas;
        }
    }

    private void Awake()
    {
        CacheAllWindows();
    }

    private void CacheAllWindows()
    {
        foreach (UIWindowType windowType in System.Enum.GetValues(typeof(UIWindowType)))
        {
            windowCache[windowType] = ResourceManager.Instance.Load($"Prefabs/UI/Window/{windowType}")?.GetComponent<WindowBase>();
            GameObject windowPrefab = windowCache[windowType]?.gameObject;
            if (windowPrefab != null)
            {
                GameObject windowInstance = Instantiate(windowPrefab, transform);
                windowInstance.SetActive(false); // ó������ ��Ȱ��ȭ ���·� �Ӵϴ�.
                WindowBase windowBase = windowInstance.GetComponent<WindowBase>();
                if (windowBase != null)
                {
                    windowCache[windowType] = windowBase;
                }
            }
            else
            {
                //Debug.LogError($"Failed to load window prefab for {windowType}");
            }
        }
    }

    public T ShowWindow<T>(UIWindowType type) where T : WindowBase
    {
        if (windowCache.TryGetValue(type, out WindowBase window))
        {
            window.gameObject.transform.SetParent(MainCanvas.transform);
            window.Refresh();
            window.gameObject.SetActive(true);
            return window as T;
        }
        else
        {
            Debug.LogError($"Window of type {type} is not cached.");
            return null;
        }
    }

    public T ShowWindow<T>(string name = null) where T : WindowBase
    {
        return windowManager.ShowUI<T>(name, "UI/Windows/") as T;
    }

    public T ShowPopup<T>(string name = null) where T : PopupBase
    {
        return popupManager.ShowUI<T>(name, "UI/Popups/") as T;
    }

    public void HideCurrentWindow()
    {
        windowManager.HideCurrentUI();
    }

    public void HideCurrentPopup()
    {
        popupManager.HideCurrentUI();
    }

    public void GoBackWindow()
    {
        windowManager.GoBack();
    }

    public void GoBackPopup()
    {
        popupManager.GoBack();
    }
}

public class UIStackManager<T> where T : UIBase
{
    private Stack<T> uiStack = new Stack<T>();

    public U ShowUI<U>(string name, string pathPrefix) where U : T
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(U).Name;

        GameObject uiObject = ResourceManager.Instance.Load($"{pathPrefix}{name}");
        if (uiObject == null)
        {
            Debug.LogError($"Failed to load UI prefab: {pathPrefix}{name}");
            return null;
        }

        GameObject instance = GameObject.Instantiate(uiObject, UIManagerNew.Instance.transform);
        U uiElement = instance.GetComponent<U>();
        if (uiElement == null)
        {
            Debug.LogError($"UI prefab {name} does not have component {typeof(U).Name}");
            return null;
        }

        HideCurrentTopUI();

        uiStack.Push(uiElement);
        uiElement.Show();

        return uiElement;
    }

    private void HideCurrentTopUI()
    {
        if (uiStack.Count > 0)
        {
            T currentUI = uiStack.Peek();
            currentUI.Hide();
        }
    }

    public void HideCurrentUI()
    {
        if (uiStack.Count > 0)
        {
            T currentUI = uiStack.Pop();
            currentUI.Hide();

            ShowPreviousTopUI();
        }
    }

    private void ShowPreviousTopUI()
    {
        if (uiStack.Count > 0)
        {
            T previousUI = uiStack.Peek();
            previousUI.Show();
        }
    }

    public void GoBack()
    {
        HideCurrentUI();
    }
}