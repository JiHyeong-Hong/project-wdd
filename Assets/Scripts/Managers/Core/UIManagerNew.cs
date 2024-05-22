using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using static Define;

public class UIManagerNew : SingletonMonoBehaviour<UIManagerNew>
{
    private Dictionary<UIWindowType, WindowBase> windowCache = new Dictionary<UIWindowType, WindowBase>();
    private UIStackManager<WindowBase> windowManager = new UIStackManager<WindowBase>();
    private UIStackManager<PopupBase> popupManager = new UIStackManager<PopupBase>();


    private void CacheAllWindows()
    {
        foreach (UIWindowType windowType in System.Enum.GetValues(typeof(UIWindowType)))
        {
            GameObject windowPrefab = ResourceManager.Instance.Load($"UI/Windows/{windowType}");
            /*Managers.Resource.Load($"UI/Windows/{windowType}");*/
            if (windowPrefab != null)
            {
                GameObject windowInstance = Instantiate(windowPrefab, transform);
                windowInstance.SetActive(false); // 처음에는 비활성화 상태로 둡니다.
                WindowBase windowBase = windowInstance.GetComponent<WindowBase>();
                if (windowBase != null)
                {
                    windowCache[windowType] = windowBase;
                }
            }
            else
            {
                Debug.LogError($"Failed to load window prefab for {windowType}");
            }
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

    public T ShowUI<U>(string name, string pathPrefix) where U : T
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(U).Name;

        GameObject uiObject = Managers.Resource.Instantiate($"{pathPrefix}{name}", UIManagerNew.Instance.transform);
        U uiElement = Util.GetOrAddComponent<U>(uiObject);

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