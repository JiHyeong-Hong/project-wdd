using System.Collections;
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

    // 삭제예정
    // UI창이 캐싱되었는지 확인한다. @홍지형 240804
    public Dictionary<UIWindowType, WindowBase> WindowCache { get => windowCache; set => windowCache = value; }
    public bool IsWindowCached(UIWindowType windowType)
    {
        if (windowCache[windowType].isCached == true)
        {
            return true;
        }
        else
            return false;
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
                windowInstance.SetActive(false); // 처음에는 비활성화 상태로 둡니다.
                WindowBase windowBase = windowInstance.GetComponent<WindowBase>();
                if (windowBase != null)
                {
                    windowBase.isCached = true;
                    windowCache[windowType] = windowBase;
                }
            }
            else
            {
                //Debug.LogError($"Failed to load window prefab for {windowType}");
            }
        }
    }

    // 사용X, 삭제예정
    //public T ShowWindow<T>(UIWindowType type) where T : WindowBase
    //{
    //    if (windowCache.TryGetValue(type, out WindowBase window))
    //    {
    //        window.gameObject.transform.SetParent(MainCanvas.transform);
    //        window.Refresh();
    //        window.gameObject.SetActive(true);
    //        window.Show(); //@홍지형
    //        return window as T;
    //    }
    //    else
    //    {
    //        Debug.LogError($"Window of type {type} is not cached.");
    //        return null;
    //    }
    //}

    //public T HideWindow<T>(UIWindowType type) where T : WindowBase
    //{
    //    if (windowCache.TryGetValue(type, out WindowBase window))
    //    {
    //        window.Hide();
    //        return window as T;
    //    }
    //    else
    //    {
    //        Debug.LogError($"Window of type {type} is not cached.");
    //        return null;
    //    }
    //}

    // 논캐싱, 상속된 OnShow() 호출
    public T ShowWindow<T>(bool isGamePaused, string name = null) where T : WindowBase
    {
        // TODO: 일시정지 파라미터 추가 240814
        return windowManager.ShowUI<T>(name, "Prefabs/UI/Window/", isGamePaused) as T;
    }

    // public T ShowPopup<T>(string name = null, bool isGamePause) where T : PopupBase
    public T ShowPopup<T>(bool isGamePaused, string name = null) where T : PopupBase
    {
        // TODO: 일시정지 파라미터 추가 240814
        return popupManager.ShowUI<T>(name, "Prefabs/UI/Popup/", isGamePaused) as T;
    }

    public IEnumerator DelayShowWindow<T>(float timer, bool isGamePaused, string name = null) where T : WindowBase
    {
        yield return new WaitForSeconds(timer);

        windowManager.ShowUI<T>(name, "Prefabs/UI/Window/", isGamePaused);
    }
    public IEnumerator DelayShowPopup<T>(float timer, bool isGamePaused, string name = null) where T : PopupBase
    {
        yield return new WaitForSeconds(timer);

        popupManager.ShowUI<T>(name, "Prefabs/UI/Popup/", isGamePaused);
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

    public U ShowUI<U>(string name, string pathPrefix, bool isGamePaused) where U : T
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(U).Name;

        GameObject uiObject = ResourceManager.Instance.Load($"{pathPrefix}{name}");
        if (uiObject == null)
        {
            Debug.LogError($"Failed to load UI prefab: {pathPrefix}{name}");
            return null;
        }

        // GameObject instance = GameObject.Instantiate(uiObject, UIManagerNew.Instance.transform); // 삭제예정
        GameObject instance = GameObject.Instantiate(uiObject, UIManagerNew.Instance.MainCanvas.transform);
        U uiElement = instance.GetComponent<U>();
        if (uiElement == null)
        {
            Debug.LogError($"UI prefab {name} does not have component {typeof(U).Name}");
            return null;
        }

        HideCurrentTopUI();

        uiStack.Push(uiElement);
        uiElement.Show(isGamePaused);

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