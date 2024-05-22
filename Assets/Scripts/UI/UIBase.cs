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

    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
}

public abstract class WindowBase : UIBase
{
    // ������ ���� ����� ���⿡ �߰��� �� �ֽ��ϴ�.
}

public abstract class PopupBase : UIBase
{
    // �˾� ���� ����� ���⿡ �߰��� �� �ֽ��ϴ�.
}