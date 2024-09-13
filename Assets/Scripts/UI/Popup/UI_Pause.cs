using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 레거시 코드, 미사용 @홍지형 240912
public class UI_Pause : UI_Popup
{
    enum Texts
    {
        MonsterKillCount,
    }

    enum Images
    {
        BackGround
    }

    enum Buttons
    {
        ContinueButton,
        HomeButton,
        ExitBtn,
        AddBtn,
        MuteButton,
    }

    private TextMeshProUGUI _monsterKillCount;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;       

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));

        _monsterKillCount = GetText((int)Texts.MonsterKillCount);

        GetImage((int)Images.BackGround).gameObject.BindEvent(OnClickReturnButton);
        GetButton((int)Buttons.ContinueButton).gameObject.BindEvent(OnClickReturnButton);
        GetButton((int)Buttons.ExitBtn).gameObject.BindEvent(OnClickReturnButton);
        GetButton((int)Buttons.MuteButton).gameObject.BindEvent(OnClickToggleSoundButton);

        RefreshUI();
        return true;
    }

    private void RefreshUI()
    {
        _monsterKillCount.text = "처치 수: " + Managers.Game.MonsterKillCount;
    }

    private void OnClickReturnButton(PointerEventData evt)
    {
        SoundManager.Instance.Play(Define.ESoundMainType.UI, Define.ESoundType.Button);
        Managers.Game.IsGamePaused = false;
        Managers.UI.ClosePopupUI(this);
    }

    private void OnClickToggleSoundButton(PointerEventData evt)
    {
        
    }
}
