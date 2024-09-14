using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    public bool isCached = false; // 캐싱되었는지 여부 확인 240804 @홍지형
       
    public virtual void Show(bool isGamePaused) // UI창마다 인게임 일시정지 여부가 다름
    {
        if (isGamePaused == true) { Managers.Game.IsGamePaused = true; } 

        gameObject.SetActive(true);
        OnShow();
    }

    public virtual void Show() // 스택에서 빼는 UI는 일시정지 여부 상관 없음.
    {       
        gameObject.SetActive(true);
        OnShow();
    }

    public virtual void Hide()
    {
        Managers.Game.IsGamePaused = false;
        gameObject.SetActive(false);
        OnHide();
    }

    public virtual void Hide(bool isGamePaused) // UI창마다 인게임 일시정지 여부가 다름
    {
        if (isGamePaused == true) { Managers.Game.IsGamePaused = false; }
        gameObject.SetActive(false);
        OnHide();
    }


    public virtual void Refresh() 
    {
        if (this.ToString().Contains("InGameWindow")) { return; }                           // Ingame UI 는 예외 위치 설정.
        else { gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; }
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