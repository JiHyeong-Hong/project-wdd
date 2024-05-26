using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    int _order = 10;

    public Dictionary<Define.UIWindowType, UIWindow> windowDic = new();
    Stack<MonoBehaviour> windowStack = new Stack<MonoBehaviour>();
    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    UI_Scene _sceneUI = null;

    Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();

    public GameObject Joystick { get; private set; }

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


    public void SetJoyStick(GameObject joyStick)
    {
        Joystick = joyStick;
    }

    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

	public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = Managers.Resource.Instantiate($"UI/WorldSpace/{name}");
		if (parent != null)
			go.transform.SetParent(parent);

        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

		return Util.GetOrAddComponent<T>(go);
	}

	public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{name}");
		if (parent != null)
			go.transform.SetParent(parent);

		return Util.GetOrAddComponent<T>(go);
	}

    public T ShowBaseUI<T>(string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/{name}");
        T baseUI = Util.GetOrAddComponent<T>(go);

        go.transform.SetParent(Root.transform);

        return baseUI;
    }

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
		T sceneUI = Util.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI;

		go.transform.SetParent(Root.transform);

		return sceneUI;
	}

	public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
        T popup = Util.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);

        go.transform.SetParent(Root.transform);

		return popup;
    }

    public T ShowMessageBox<T>(string name = null) where T : MonoBehaviour
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Common/{name}", MainCanvas);
        T messageBox = Util.GetOrAddComponent<T>(go);

        go.transform.SetParent(MainCanvas);

        return messageBox;
    }

    public T ShowWindowUI<T>(Define.UIWindowType type) where T : UIWindow
    {
        if (!windowDic.ContainsKey(type))
        {
            GameObject go = Managers.Resource.Instantiate($"UI/Window/{typeof(T).Name}", MainCanvas);
            T window = Util.GetOrAddComponent<T>(go);
            windowDic.Add(type, window);
            go.transform.SetParent(MainCanvas);
        }

        return windowDic[type] as T;
    }

    //public T ShowWindowUI<T>(string name = null) where T : MonoBehaviour
    //{
    //    // 작업중 05.03 
    //    // window가 dictionary에 없으면 생성작업중
    //    if (string.IsNullOrEmpty(name))
    //        name = typeof(T).Name;

    //    GameObject go = Managers.Resource.Instantiate($"Window/{name}");
    //    T window = Util.GetOrAddComponent<T>(go);
    //    windowStack.Push(window);

    //    go.transform.SetParent(MainCanvas);

    //    return window;
    //}

    public void ClosePopupUI(UI_Popup popup)
    {
		if (_popupStack.Count == 0)
			return;

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }

        ClosePopupUI();
    }

    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;
        _order--;
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }

    public void Clear()
    {
        CloseAllPopupUI();
        _sceneUI = null;
    }
}
