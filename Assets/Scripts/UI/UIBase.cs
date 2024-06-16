using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    public virtual void Show()
    {
        gameObject.SetActive(true);
        OnShow();
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
        OnHide();
    }

    public virtual void Refresh() 
    {
        // gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // 홍지형 IngameUI 위치문제로 삭제. 240616
    }

    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
}

public abstract class WindowBase : UIBase
{
    // 윈도우 전용 기능을 여기에 추가할 수 있습니다.
}

public abstract class PopupBase : UIBase
{
    // 팝업 전용 기능을 여기에 추가할 수 있습니다.
}