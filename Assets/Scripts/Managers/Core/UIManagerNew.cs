using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class UIManagerNew : SingletonMonoBehaviour<UIManagerNew>
{
    private UIStackManager<WindowBase> windowManager = new UIStackManager<WindowBase>();
    private UIStackManager<PopupBase> popupManager = new UIStackManager<PopupBase>();

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

        GameObject go = Managers.Resource.Instantiate($"{pathPrefix}{name}", UIManagerNew.Instance.transform);
        U uiElement = Util.GetOrAddComponent<U>(go);

        if (uiStack.Count > 0)
        {
            T currentUI = uiStack.Peek();
            currentUI.Hide();
        }

        uiStack.Push(uiElement);
        uiElement.Show();

        return uiElement;
    }

    public void HideCurrentUI()
    {
        if (uiStack.Count > 0)
        {
            T currentUI = uiStack.Pop();
            currentUI.Hide();

            if (uiStack.Count > 0)
            {
                T previousUI = uiStack.Peek();
                previousUI.Show();
            }
        }
    }

    public void GoBack()
    {
        HideCurrentUI();
    }
}