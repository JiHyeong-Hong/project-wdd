using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_GameScene : UI_Scene
{
    enum Texts
    {
        Timer,
        Gold,
    }

    enum Images
    {
        HPBar,
        ExpBar,
    }

    enum Buttons
    {
        PauseBtn,
    }

    private float _timeElapsed;
    private TextMeshProUGUI _timerText;
    private Image _expBar;
    private Image _hpBar;

    // 보스체력바 <- 보스 생기면

    // 골드 획득량 (수정 가능성 있음 우선순위 제일 뒤)
    private TextMeshProUGUI _goldText;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));        

        _timerText = GetText((int)Texts.Timer);
        _goldText = GetText((int)Texts.Gold);
        _hpBar = GetImage((int)Images.HPBar);
        _expBar = GetImage((int)Images.ExpBar);

        GetButton((int)Buttons.PauseBtn).gameObject.BindEvent(OnClickPauseButton, Define.EUIEvent.Click);

        Managers.Game.OnUIRefreshed -= RefreshUI;
        Managers.Game.OnUIRefreshed += RefreshUI;

        return true;
    }

    void Update()
    {
        if (!Managers.Game.IsGamePaused)
        {
            UpdateTimer();
        }
    }

    private void UpdateTimer()
    {
        _timeElapsed += Time.deltaTime;
        int minutes = (int)(_timeElapsed / 60);
        int seconds = (int)(_timeElapsed % 60);
        _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        Managers.Game.CurrentTime = _timeElapsed;
    }

    private void RefreshUI()
    {
        _hpBar.fillAmount = (float)Managers.Object.Hero.Hp / Managers.Object.Hero.MaxHp;
        _expBar.fillAmount = (float)Managers.Object.Hero.Exp / Managers.Object.Hero.MaxExp;
        _goldText.text = "골드: " + Managers.Game.Gold;
    }

    private void OnClickPauseButton(PointerEventData evt)
    {
        Managers.Game.IsGamePaused = true;
        Managers.UI.ShowPopupUI<UI_Pause>();
    }
}
